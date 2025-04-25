
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
using Shared.Models;


namespace Server.Repositories;

public class AccountRepository(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, PasswordManagerDbContext passwordManagerDbContext, HttpClient httpClient) : IAccountRepository
{

    public async Task<LoginResponse> LoginAccount(LoginDTO loginDTO)
    {
        if (loginDTO is null)
        {
            return new LoginResponse(false, null!, "Login container is empty");
        }

        var response = await httpClient.PostAsJsonAsync<LoginDTO>("https://dotnetusermanagementsystem-production.up.railway.app/api/UMS/get-user-credentials?appName=PasswordManager", loginDTO);

        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);

        // Access root properties
        var root = doc.RootElement;

        string username = root.GetProperty("username").GetString();
        string email = root.GetProperty("email").GetString();
        string userId = root.GetProperty("userId").GetString();

        var roles = root.GetProperty("roles")
            .EnumerateArray()
            .Select(r => r.GetProperty("role").GetString())
            .ToList();

        // add user to the user table if they arent in it already
        if (await passwordManagerDbContext.PasswordmanagerUsers.FirstOrDefaultAsync(u => u.UmsUserid == userId) is null)
        {
            try
            {
                await passwordManagerDbContext.PasswordmanagerUsers.AddAsync(new PasswordmanagerUser
                {
                    UmsUserid = userId!
                    ,
                    Email = email!
                    ,
                    Datecreated = DateTime.Now
                    ,
                    Datelastlogin = DateTime.Now
                    ,
                    Dateretired = null
                });
                await passwordManagerDbContext.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                return new LoginResponse(false, null!, ex.Message);
            }
        }

        var passwordDbUser = await passwordManagerDbContext.PasswordmanagerUsers.FirstAsync(u => u.UmsUserid == userId);
        passwordDbUser.Datelastlogin = DateTime.Now;
        await passwordManagerDbContext.SaveChangesAsync();

        string token = GenerateToken(passwordDbUser.Id, username, email, roles.First());

        return new LoginResponse(true, token!, "Login completed");
    }

    public async Task<GeneralResponse> Logout(int userId)
    {
        try
        {
            var stockUser = await passwordManagerDbContext.PasswordmanagerUsers.FindAsync(userId);
            stockUser.Datelastlogout = DateTime.Now;
            await passwordManagerDbContext.SaveChangesAsync();
            return new GeneralResponse(Flag: true, Message: "log out success!");
        }
        catch (System.Exception ex)
        {
            return new GeneralResponse(Flag: false, Message: ex.Message);
        }

    }

    private string GenerateToken(int userId, string userName, string email, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(webHostEnvironment.IsDevelopment() ? configuration["Jwt:Key"] : Environment.GetEnvironmentVariable("Jwt_Key")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: webHostEnvironment.IsDevelopment() ? configuration["Jwt:Issuer"] : Environment.GetEnvironmentVariable("Jwt_Issuer"),
            audience: webHostEnvironment.IsDevelopment() ? configuration["Jwt:Audience"] : Environment.GetEnvironmentVariable("Jwt_Audience"),
            claims: userClaims,
            expires: JwtConfig.JWT_TOKEN_EXP_DATETIME,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}

