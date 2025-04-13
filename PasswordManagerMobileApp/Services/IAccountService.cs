
using PasswordManagerMobileApp.Models;
using static PasswordManagerMobileApp.Models.ServiceResponses;

namespace PasswordManagerMobileApp.Services;

public interface IAccountService
{
    Task<LoginResponse> LoginAsync(LoginDTO loginDTO);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
}


