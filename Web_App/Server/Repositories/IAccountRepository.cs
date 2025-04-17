

using Shared.Models;


namespace Server.Repositories;

public interface IAccountRepository
{
    Task<HandyGeneralResponse> Logout(int userId);
    Task<HandyLoginResponse> LoginAccount(LoginDTO loginDTO);
    Task<HandyGeneralResponse> CheckPassword(string email, string password);
}
