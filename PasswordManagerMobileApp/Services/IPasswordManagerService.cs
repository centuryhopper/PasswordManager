
using PasswordManagerMobileApp.Models;
using static PasswordManagerMobileApp.Models.ServiceResponses;

namespace PasswordManagerMobileApp.Services;

public interface IPasswordManagerService
{
    Task<IEnumerable<PasswordAccountDTO>> GetPasswordAccountsAsync();
}


