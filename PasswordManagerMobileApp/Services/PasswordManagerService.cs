

using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Maui.Storage;
using PasswordManagerMobileApp.Models;
using static PasswordManagerMobileApp.Models.ServiceResponses;

namespace PasswordManagerMobileApp.Services;


public class PasswordManagerService : IPasswordManagerService
{
    private readonly HttpClient httpClient;

    public PasswordManagerService(HttpClient httpClient)
    {
        this.httpClient = httpClient;        
    }

    public async Task<IEnumerable<PasswordAccountDTO>> GetPasswordAccountsAsync()
    {
        var jwt = Preferences.Get(JwtConfig.JWT_TOKEN_NAME, null);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        var records = await httpClient.GetFromJsonAsync<IEnumerable<PasswordAccountDTO>>("https://mypasswordmanager-production-b6be.up.railway.app/api/PasswordManager/passwords");

        return records ?? Enumerable.Empty<PasswordAccountDTO>();
    }
}