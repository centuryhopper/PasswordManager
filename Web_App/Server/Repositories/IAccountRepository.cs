

using Shared.Models;


namespace Server.Repositories;

public interface IAccountRepository
{
    Task<GeneralResponse> Logout(int userId);
    Task<LoginResponse> LoginAccount(LoginDTO loginDTO);
}
