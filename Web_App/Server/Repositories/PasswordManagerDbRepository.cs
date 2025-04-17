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


namespace Server.Repositories;

public class PasswordManagerDbRepository(EncryptionContext encryptionContext, ILogger<PasswordManagerDbRepository> logger, PasswordManagerDbContext passwordManagerDbContext) : IPasswordManagerDbRepository
{
    public async Task<PasswordAccountDTO?> GetPasswordRecordAsync(int passwordRecordId)
    {
        var accountModel = await passwordManagerDbContext.PasswordmanagerAccounts.FindAsync(passwordRecordId);
        accountModel.Password = encryptionContext.Decrypt(Convert.FromBase64String(accountModel.Password)).Replace(",", "$");
        return accountModel?.ToPasswordManagerAccountDTO();
    }

    public int AccountsCount(int UserId, string title)
    {
        var cnt = passwordManagerDbContext.PasswordmanagerAccounts.Where(a => a.Userid == UserId && a.Title.ToLower().Contains(title)).Count();
        return cnt;
    }

    public async Task<GeneralResponse> UploadCsvAsync(IEnumerable<PasswordAccountDTO> uploadedResults, int userId)
    {
        try
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
        }
        catch (Exception ex)
        {
            return new GeneralResponse(Flag: false, Message: ex.Message);
        }

        return new GeneralResponse(Flag: true, Message: "File uploaded!");
    }

    // public async Task<GeneralResponse> UploadCsvAsync(IFormFile file, int userId)
    // {
    //     // set up csv helper and read file
    //     var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    //     {
    //         HasHeaderRecord = true,
    //         // set to null to allow files with only title, usernam, and password headers to be uploaded
    //         HeaderValidated = null,
    //         MissingFieldFound = null,
    //     };


    //     using var streamReader = new StreamReader(file.OpenReadStream());
    //     using var csvReader = new CsvReader(streamReader, config);
    //     IAsyncEnumerable<PasswordAccountDTO> records;

    //     try
    //     {
    //         csvReader.Context.RegisterClassMap<PasswordsMapper>();
    //         records = csvReader.GetRecordsAsync<PasswordAccountDTO>();

    //         await foreach (var record in records)
    //         {
    //             await CreateAsync(new PasswordAccountDTO
    //             {
    //                 UserId = userId,
    //                 Title = record.Title,
    //                 Username = record.Username,
    //                 Password = record.Password,
    //                 CreatedAt = DateTime.Now,
    //                 LastUpdatedAt = DateTime.Now,
    //             });
    //         }
    //     }
    //     catch (CsvHelperException ex)
    //     {
    //         return new GeneralResponse(Flag: false, Message: ex.Message);
    //     }

    //     return new GeneralResponse(Flag: true, Message: "File uploaded!");

    // }

    public async Task<IEnumerable<PasswordAccountDTO>> GetAllPasswordRecordsAsync(int userId)
    {
        var results = await passwordManagerDbContext.PasswordmanagerAccounts.AsNoTracking().Where(a => a.Userid == userId).ToListAsync();

        if (!results.Any())
        {
            return Enumerable.Empty<PasswordAccountDTO>();
        }

        return results.Select(m =>
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
    }

    public async Task<IEnumerable<GeneralResponseWithPayload>> CreateMultipleAsync(IEnumerable<PasswordAccountDTO> dtos)
    {
        List<GeneralResponseWithPayload> responses = [];
        foreach (var dto in dtos)
        {
            responses.Add(await CreateAsync(dto));
        }
        return responses;
    }

    public async Task<GeneralResponseWithPayload> CreateAsync(PasswordAccountDTO model)
    {
        model.Password = Convert.ToBase64String(encryptionContext.Encrypt(model.Password!));
        model.CreatedAt = DateTime.Now;
        model.LastUpdatedAt = DateTime.Now;
        try
        {
            var record = model.ToPasswordManagerAccount();
            await passwordManagerDbContext.PasswordmanagerAccounts.AddAsync(record);
            await passwordManagerDbContext.SaveChangesAsync();
            return new GeneralResponseWithPayload(Flag: true, Message: "Password record created!", record.Id.ToString());
        }
        catch (System.Exception ex)
        {
            return new GeneralResponseWithPayload(Flag: false, Message: ex.Message, "");
        }

    }

    public async Task<GeneralResponse> UpdateAsync(PasswordAccountDTO model)
    {
        var dbModel = await passwordManagerDbContext.PasswordmanagerAccounts.FindAsync(model.Id);
        try
        {
            dbModel!.LastUpdatedAt = DateTime.Now;
            dbModel.Title = model.Title;
            dbModel.Username = model.Username;
            dbModel.Password = Convert.ToBase64String(encryptionContext.Encrypt(model.Password));
            await passwordManagerDbContext.SaveChangesAsync();
        }
        catch (System.Exception ex)
        {
            return new GeneralResponse(Flag: false, Message: ex.Message);
        }
        return new GeneralResponse(Flag: true, Message: "Password Record Updated!");
    }

    public async Task<GeneralResponse> DeleteAsync(int passwordRecordId)
    {
        var queryModel = await passwordManagerDbContext.PasswordmanagerAccounts.FindAsync(passwordRecordId);

        if (queryModel is null)
        {
            return new GeneralResponse(Flag: false, Message: "This record doesn't exist.");
        }

        passwordManagerDbContext.PasswordmanagerAccounts.Remove(queryModel!);

        try
        {
            await passwordManagerDbContext.SaveChangesAsync();
        }
        catch (System.Exception ex)
        {
            return new GeneralResponse(Flag: false, Message: ex.Message);
        }

        return new GeneralResponse(Flag: true, Message: "Password Record Deleted!");
    }

}

