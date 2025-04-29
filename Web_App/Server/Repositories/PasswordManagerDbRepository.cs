using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Server.Contexts;
using Server.Repositories;
using Shared.Models;
using Server.Entities;
using Server.Utils;
using Shared.Models;

using System.Security.Claims;
using LanguageExt;


namespace Server.Repositories;

public class PasswordManagerDbRepository(EncryptionContext encryptionContext, ILogger<PasswordManagerDbRepository> logger, PasswordManagerDbContext passwordManagerDbContext) : IPasswordManagerDbRepository
{
    public async Task<Option<PasswordAccountDTO>> GetPasswordRecordAsync(int passwordRecordId) => await TryAsync(async () =>
        {
            var record = await passwordManagerDbContext.PasswordmanagerAccounts.FindAsync(passwordRecordId);

            return Optional(record).Map(model =>
            {
                model.Password = encryptionContext
                    .Decrypt(Convert.FromBase64String(model.Password))
                    .Replace(",", "$");
                return model.ToPasswordManagerAccountDTO();
            });
        }).Match(
            Succ: val => val,
            Fail: _ => Option<PasswordAccountDTO>.None
        );

    public async Task<GeneralResponse> UploadCsvAsync(IEnumerable<PasswordAccountDTO> uploadedResults, int userId) => await TryAsync(async () =>
        {
            foreach (var uploadedResult in uploadedResults)
            {
                var response = await CreateAsync(new PasswordAccountDTO
                {
                    UserId = userId,
                    Title = uploadedResult.Title,
                    Username = uploadedResult.Username,
                    Password = uploadedResult.Password,
                    CreatedAt = DateTime.Now,
                    LastUpdatedAt = DateTime.Now,
                });
            }
            return new GeneralResponse(Flag: true, Message: "File uploaded!");
        })
        .Match(
            Succ: res => res,
            Fail: ex => new GeneralResponse(false, ex.Message)
        );

    public async Task<Option<IEnumerable<PasswordAccountDTO>>> GetAllPasswordRecordsAsync(int userId) => await TryAsync(async () =>
        {
            var results = await passwordManagerDbContext.PasswordmanagerAccounts.AsNoTracking().Where(a => a.Userid == userId).ToListAsync();

            return Optional(results).Map(results =>
            {
                return !results.Any() ? Enumerable.Empty<PasswordAccountDTO>() : results.Select(m =>
                {
                    return new PasswordAccountDTO
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Username = m.Username,
                        Password = encryptionContext.Decrypt(Convert.FromBase64String(m.Password)).Replace(",", "$"),
                        UserId = m.Userid,
                        CreatedAt = m.CreatedAt,
                        LastUpdatedAt = m.LastUpdatedAt
                    };
                });
            });
        }).Match(
            Succ: val => val,
            Fail: _ => None
        );

    public async Task<IEnumerable<GeneralResponseWithPayload>> CreateMultipleAsync(IEnumerable<PasswordAccountDTO> dtos)
    {
        List<GeneralResponseWithPayload> responses = [];
        foreach (var dto in dtos)
        {
            responses.Add(await CreateAsync(dto));
        }
        return responses;
    }

    public async Task<GeneralResponseWithPayload> CreateAsync(PasswordAccountDTO model) => await TryAsync(async () =>
        {
            model.Password = Convert.ToBase64String(encryptionContext.Encrypt(model.Password!));
            model.CreatedAt = DateTime.Now;
            model.LastUpdatedAt = DateTime.Now;

            var record = model.ToPasswordManagerAccount();
            await passwordManagerDbContext.PasswordmanagerAccounts.AddAsync(record);
            await passwordManagerDbContext.SaveChangesAsync();
            return new GeneralResponseWithPayload(Flag: true, Message: "Password record created!", record.Id.ToString());
        })
        .Match(
            Succ: res => res,
            Fail: ex => new GeneralResponseWithPayload(Flag: false, Message: ex.Message, "")
        );

    public async Task<GeneralResponse> UpdateAsync(PasswordAccountDTO model)
        => await TryAsync(async () =>
        {
            var dbModel = await passwordManagerDbContext.PasswordmanagerAccounts.FindAsync(model.Id);
            dbModel!.LastUpdatedAt = DateTime.Now;
            dbModel.Title = model.Title;
            dbModel.Username = model.Username;
            dbModel.Password = Convert.ToBase64String(encryptionContext.Encrypt(model.Password));
            await passwordManagerDbContext.SaveChangesAsync();

            return new GeneralResponse(Flag: true, Message: "Password Record Updated!");
        })
        .Match(
            Succ: res => res,
            Fail: ex => new GeneralResponse(Flag: false, Message: ex.Message)
        );



    public async Task<GeneralResponse> DeleteAsync(int passwordRecordId) => await TryAsync(async () =>
        {
            var queryModel = await passwordManagerDbContext.PasswordmanagerAccounts.FindAsync(passwordRecordId);
            if (queryModel is null)
            {
                throw new Exception("This record doesn't exist.");
            }
            passwordManagerDbContext.PasswordmanagerAccounts.Remove(queryModel!);
            await passwordManagerDbContext.SaveChangesAsync();
            return new GeneralResponse(Flag: true, Message: "Password Record Deleted!");
        })
        .Match(
            Succ: _ => _,
            Fail: ex => new GeneralResponse(Flag: false, Message: ex.Message)
        );
}
































// public int AccountsCount(int UserId, string title)
// {
//     var cnt = passwordManagerDbContext
//         .PasswordmanagerAccounts
//         .Where(a => a.Userid == UserId && a.Title.ToLower()
//         .Contains(title)).Count();
//     return cnt;
// }