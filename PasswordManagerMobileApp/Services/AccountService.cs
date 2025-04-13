

using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Maui.Storage;
using PasswordManagerMobileApp.Models;
using static PasswordManagerMobileApp.Models.ServiceResponses;

namespace PasswordManagerMobileApp.Services;


public class AccountService : IAccountService
{
    private readonly HttpClient httpClient;

    public AccountService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        await Task.Delay(3000);
        return true;
    }

    public async Task<LoginResponse> LoginAsync(LoginDTO loginDTO)
    {
        return new LoginResponse(true, "", "test login successful");
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/Account/login", loginDTO);
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(loginResponse!.Message);
            }
            if (string.IsNullOrWhiteSpace(loginResponse.Token))
            {
                throw new Exception("Couldn't get a token");
            }

            if (loginDTO.RememberMe)
            {
                await SecureStorage.SetAsync(JwtConfig.JWT_TOKEN_NAME, loginResponse!.Token);
            }

            return loginResponse!;
        }
        catch (System.Exception ex)
        {
            return new LoginResponse(Flag: false, Token: "", Message: ex.Message);
        }
    }

    public async Task LogoutAsync()
    {

    }
}