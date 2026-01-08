
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web.LayoutRenderers;
using Server.Contexts;
using Server.Entities;
using Server.Utils;
using Shared.Models;
using LanguageExt;
using static LanguageExt.Prelude;


namespace Server.Repositories;

public partial class AccountRepository(ConfigurationProvider configurationProvider, IWebHostEnvironment webHostEnvironment, PasswordManagerDbContext passwordManagerDbContext, HttpClient httpClient) : IAccountRepository
{
    public async Task<GeneralResponse> CheckPasswordManagerUser(LoginDTO loginDTO)
    {
        var result = await httpClient.GetAsync($"https://dotnetusermanagementsystem-production.up.railway.app/api/UMS/check-user/{loginDTO.Email}/{loginDTO.Password}");
        if (!result.IsSuccessStatusCode)
        {
            return new GeneralResponse(false, "Invalid user credentials");
        }
        return new GeneralResponse(true, "User verified");
    }

    public async Task<LoginResponse> LoginAccount(LoginDTO loginDTO)
    {
        var result =
            from dto in ValidateLoginDTO(loginDTO)
            from response in SendLoginRequest(dto)
            from loginData in ParseLoginResponse(response)
            from user in UpdateOrCreateUser(loginData)
            select GenerateToken(user.Id, user.UmsUserid, user.Email, user.Roles.First());

        return await result.Match(
            Right: token => new LoginResponse(true, token, "Login completed"),
            Left: error => new LoginResponse(false, null!, error)
        );
    }

    public async Task<LoginResponse> LoginAccountWith2FA(string email, string code)
    {
        var result =
            from loginData in VerifyTwoFactorCode(email, code)
            from user in UpdateOrCreateUser(loginData)
            select GenerateToken(user.Id, user.UmsUserid, user.Email, user.Roles.First());

        return await result.Match(
            Right: token => new LoginResponse(true, token, "Login completed"),
            Left: error => new LoginResponse(false, null!, error)
        );
    }

    public async Task<GeneralResponse> Logout(int userId)
    {
        return await TryAsync(async () =>
            {
                var user = await passwordManagerDbContext.PasswordmanagerUsers.FindAsync(userId);
                if (user is null) throw new Exception("User not found");

                user.Datelastlogout = DateTime.Now;
                await passwordManagerDbContext.SaveChangesAsync();
                return new GeneralResponse(true, "Log out success!");
            })
            .Match(
                Succ: res => res,
                Fail: ex => new GeneralResponse(false, ex.Message)
            );
    }

    private string GenerateToken(int userId, string userName, string email, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configurationProvider.JwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
        };

        var token = new JwtSecurityToken(
            issuer: configurationProvider.JwtIssuer,
            audience: configurationProvider.JwtAudience,
            claims: userClaims,
            expires: JwtConfig.JWT_TOKEN_EXP_DATETIME,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}

// 2fa helper methods
public partial class AccountRepository
{
    public async Task<bool> GetTwoFactorStatus(string email)
    {
        var response = await httpClient.GetAsync("https://dotnetusermanagementsystem-production.up.railway.app/api/UMS/get-2fa-status/" + email);

        using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        var root = doc.RootElement;
        var twoFactorEnabled = root.GetProperty("twoFactorEnabled").GetBoolean();

        return twoFactorEnabled;
    }

    public async Task<(string, bool)> SendTwoFactorCode(string email)
    {
        var response = await httpClient.GetAsync("https://dotnetusermanagementsystem-production.up.railway.app/api/UMS/send-2fa-code/" + email);
        using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        var root = doc.RootElement;
        string msg = root.GetProperty("message").GetString()!;
        var flag = root.GetProperty("flag").GetBoolean()!;
        return (msg, flag);
    }

    public EitherAsync<string, (string, string, string, List<string>)> VerifyTwoFactorCode(string email, string code) => TryAsync(async () =>
    {
        var query = $"https://dotnetusermanagementsystem-production.up.railway.app/api/UMS/verify-2fa?email={email}&code={code}";
        var response = await httpClient.GetAsync(query);
        using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        var root = doc.RootElement;
        string userId = root.GetProperty("userId").GetString()!;
        string username = root.GetProperty("username").GetString()!;
        List<string> userRoles = root.GetProperty("userRoles")
            .EnumerateArray() // get each element in the JSON array
            .Select(r => r.GetString() ?? string.Empty)       // convert JsonElement to string
            .Where(s => !string.IsNullOrWhiteSpace(s))        // optional: filter out empty/null
            .ToList();

        return (userId, username, email, userRoles);
    }).ToEither(ex => ex.Message);

}

// login helper methods
public partial class AccountRepository
{
    private EitherAsync<string, LoginDTO> ValidateLoginDTO(LoginDTO loginDTO) =>
        loginDTO is not null
            ? RightAsync<string, LoginDTO>(loginDTO)
            : LeftAsync<string, LoginDTO>("Login container is empty");

    private EitherAsync<string, HttpResponseMessage> SendLoginRequest(LoginDTO loginDTO) =>
       TryAsync(async () =>
       {
           var response = await httpClient.PostAsJsonAsync("https://dotnetusermanagementsystem-production.up.railway.app/api/UMS/get-user-credentials?appName=PasswordManager", loginDTO);
           response.EnsureSuccessStatusCode();
           return response;
       })
       .ToEither(ex => ex.Message);

    private static EitherAsync<string, (string userId, string username, string email, List<string> roles)> ParseLoginResponse(HttpResponseMessage response) =>
        TryAsync(async () =>
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            var root = doc.RootElement;

            var userId = root.GetProperty("userId").GetString() ?? throw new Exception("UserId missing");
            var username = root.GetProperty("username").GetString() ?? throw new Exception("Username missing");
            var email = root.GetProperty("email").GetString() ?? throw new Exception("Email missing");

            var roles = root.GetProperty("roles")
                .EnumerateArray()
                .Select(r => r.GetProperty("role").GetString() ?? "")
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .ToList();

            return (userId, username, email, roles);
        })
        .ToEither(ex => ex.Message);


    private EitherAsync<string, PasswordmanagerUserDTO> UpdateOrCreateUser((string userId, string username, string email, List<string> roles) loginData) =>
        TryAsync(async () =>
        {
            var (userId, username, email, roles) = loginData;

            var existingUser = await passwordManagerDbContext.PasswordmanagerUsers
                .FirstOrDefaultAsync(u => u.UmsUserid == userId);

            if (existingUser is null)
            {
                var newUser = new PasswordmanagerUser
                {
                    UmsUserid = userId,
                    Email = email,
                    Datecreated = DateTime.Now,
                    Datelastlogin = DateTime.Now,
                    Dateretired = null,
                };

                await passwordManagerDbContext.PasswordmanagerUsers.AddAsync(newUser);
                await passwordManagerDbContext.SaveChangesAsync();
                var newUserDTO = newUser.ToDTO(roles);
                return newUserDTO;
            }
            else
            {
                existingUser.Datelastlogin = DateTime.Now;
                await passwordManagerDbContext.SaveChangesAsync();
                return existingUser.ToDTO(roles);
            }
        })
        .ToEither(ex => ex.Message);
}

