using LanguageExt;
using Microsoft.AspNetCore.Http;
using Server.Contexts;
using Server.Entities;
using Shared.Models;


namespace Server.Repositories;

public interface IPasswordManagerDbRepository
{
    Task<Option<PasswordAccountDTO>> GetPasswordRecordAsync(int passwordRecordId);
    Task<Option<IEnumerable<PasswordAccountDTO>>> GetAllPasswordRecordsAsync(int userId);
    Task<GeneralResponseWithPayload> CreateAsync(PasswordAccountDTO model);
    Task<IEnumerable<GeneralResponseWithPayload>> CreateMultipleAsync(IEnumerable<PasswordAccountDTO> passwordsToAdd);
    Task<GeneralResponse> UpdateAsync(PasswordAccountDTO model);
    Task<GeneralResponse> DeleteAsync(int passwordRecordId);
    Task<GeneralResponse> UploadCsvAsync(IEnumerable<PasswordAccountDTO> uploadedResults, int userId);
}


