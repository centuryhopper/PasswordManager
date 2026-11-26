

using LanguageExt;
using Shared.Models;


namespace Server.Repositories;

public interface IAccountRepository
{
    Task<GeneralResponse> Logout(int userId);
    Task<LoginResponse> LoginAccount(LoginDTO loginDTO);
    Task<LoginResponse> LoginAccountWith2FA(string email, string code);
    Task<(string, bool)> SendTwoFactorCode(string email);
    Task<GeneralResponse> CheckPasswordManagerUser(LoginDTO loginDTO);
    Task<bool> GetTwoFactorStatus(string email);
    EitherAsync<string, (string userId, string username, string email, List<string> roles)> VerifyTwoFactorCode(string email, string code);
}
