using Microsoft.AspNetCore.Http;
using Server.Contexts;
using Server.Entities;
using Shared.Models;


namespace Server.Repositories;

public interface IPasswordManagerDbRepository
{
    Task<PasswordAccountDTO?> GetPasswordRecordAsync(int passwordRecordId);
    Task<IEnumerable<PasswordAccountDTO>> GetAllPasswordRecordsAsync(int userId);
    Task<HandyGeneralResponseWithPayload> CreateAsync(PasswordAccountDTO model);
    Task<IEnumerable<HandyGeneralResponseWithPayload>> CreateMultipleAsync(IEnumerable<PasswordAccountDTO> passwordsToAdd);
    Task<HandyGeneralResponse> UpdateAsync(PasswordAccountDTO model);
    Task<HandyGeneralResponse> DeleteAsync(int passwordRecordId);
    Task<HandyGeneralResponse> UploadCsvAsync(IEnumerable<PasswordAccountDTO> uploadedResults, int userId);
}


