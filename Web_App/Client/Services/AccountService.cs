

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Client.Interfaces;
using Client.Providers;
using Client.Utils;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Models;
using static Shared.Models.ServiceResponses;


namespace Client.Services;

public class AccountService : IAccountService
{
    private readonly HttpClient httpClient;
    private readonly ILocalStorageService localStorageService;
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly ISessionStorageService sessionStorageService;

    public AccountService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService, AuthenticationStateProvider authenticationStateProvider, ISessionStorageService sessionStorageService)
    {
        httpClient = httpClientFactory.CreateClient(Constants.HTTP_CLIENT);
        this.localStorageService = localStorageService;
        this.authenticationStateProvider = authenticationStateProvider;
        this.sessionStorageService = sessionStorageService;
    }

    public async Task<LoginResponse> LoginAsync(LoginDTO loginDTO)
    {
        try
        {
            //var checkUserResponse = await httpClient.PostAsJsonAsync<LoginDTO>($"api/Account/check-user", loginDTO);
//
            //var checkUser = await checkUserResponse.Content.ReadFromJsonAsync<GeneralResponse>();
//
            //if (!checkUser!.Flag)
            //{
            //    return new LoginResponse(Flag: false, Token: "", Message: checkUser.Message);
            //}
//
            //var twoFaResponse = await httpClient.GetAsync("api/Account/get-2fa-status/" + loginDTO.Email);
            //using var stream = await twoFaResponse.Content.ReadAsStreamAsync();
            //using var doc = await JsonDocument.ParseAsync(stream);
            //var root = doc.RootElement;
            //var twoFactorEnabled = root.GetProperty("twoFactorStatus").GetBoolean();
//
            //Console.WriteLine($"twoFactorEnabled: {twoFactorEnabled}");
            //// await Task.Delay(5000); // Simulate network delay for better UX testing
//
            //if (twoFactorEnabled)
            //{
            //    return new(Flag: true, Token: "", Message: "Check your email for the 2FA code.");
            //}

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
                await localStorageService.SetItemAsync(JwtConfig.JWT_TOKEN_NAME, loginResponse!.Token);
            }
            else
            {
                await sessionStorageService.SetItemAsync(JwtConfig.JWT_TOKEN_NAME, loginResponse!.Token);
            }

            ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsAuthenticated(loginResponse!.Token);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResponse!.Token);

            return loginResponse!;
        }
        catch (System.Exception ex)
        {
            return new LoginResponse(Flag: false, Token: "", Message: ex.Message);
        }
    }

    public async Task LogoutAsync()
    {
        await localStorageService.RemoveItemAsync(JwtConfig.JWT_TOKEN_NAME);
        await sessionStorageService.RemoveItemAsync(JwtConfig.JWT_TOKEN_NAME);
        ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
        httpClient.DefaultRequestHeaders.Authorization = null;
    }
}