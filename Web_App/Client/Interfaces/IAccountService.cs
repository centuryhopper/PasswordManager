
using Shared.Models;


namespace Client.Interfaces;

public interface IAccountService
{
    Task<HandyLoginResponse> LoginAsync(LoginDTO loginDTO);
    Task LogoutAsync();
}


