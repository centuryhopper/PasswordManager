

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
        return Preferences.ContainsKey(JwtConfig.JWT_TOKEN_NAME) || await SecureStorage.GetAsync(JwtConfig.JWT_TOKEN_NAME) != null;
    }

    public async Task<LoginResponse> LoginAsync(LoginDTO loginDTO)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("https://mypasswordmanager-production-b6be.up.railway.app/api/Account/login", loginDTO);
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(loginResponse!.Message);
            }
            if (string.IsNullOrWhiteSpace(loginResponse.Token))
            {
                throw new Exception("Couldn't get a token");
            }

            Preferences.Set(JwtConfig.JWT_TOKEN_NAME, loginResponse.Token);

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
        Preferences.Remove(JwtConfig.JWT_TOKEN_NAME);
        SecureStorage.Remove(JwtConfig.JWT_TOKEN_NAME);
    }
}