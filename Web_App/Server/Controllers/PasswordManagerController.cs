using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Server.Repositories;
using static Shared.Models.ServiceResponses;
using Newtonsoft.Json;

namespace Server.Controllers;


[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PasswordManagerController(ILogger<PasswordManagerController> logger, IPasswordManagerDbRepository passwordManagerAccountRepository) : ControllerBase
{
    [HttpDelete("deletePassword/{passwordRecordId:int}")]
    public async Task<IActionResult> DeletePasswordRecord(int passwordRecordId)
    {
        var response = await passwordManagerAccountRepository.DeleteAsync(passwordRecordId);

        if (!response.Flag)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("getPassword/{passwordRecordId:int}")]
    public async Task<IActionResult> GetPasswordRecord(int passwordRecordId)
    {
        var passwordRecord = await passwordManagerAccountRepository.GetPasswordRecordAsync(passwordRecordId);
        if (passwordRecord is null)
        {
            return NotFound(null);
        }
        return Ok(passwordRecord);
    }

    [AllowAnonymous]
    [HttpGet("test")]
    public IActionResult Test()
    {
        var users = new[]
        {
            new { id = "1", name = "Leanne Graham", username = "Bret", email = "Sincere@april.biz" },
            new { id = "2", name = "Ervin Howell", username = "Antonette", email = "Shanna@melissa.tv" },
            new { id = "3", name = "Clementine Bauch", username = "Samantha", email = "Nathan@yesenia.net" },
            new { id = "4", name = "Patricia Lebsack", username = "Karianne", email = "Julianne.OConner@kory.org" },
            new { id = "5", name = "Chelsey Dietrich", username = "Kamren", email = "Lucio_Hettinger@annie.ca" },
            new { id = "6", name = "Mrs. Dennis Schulist", username = "Leopoldo_Corkery", email = "Karley_Dach@jasper.info" },
            new { id = "7", name = "Kurtis Weissnat", username = "Elwyn.Skiles", email = "Telly.Hoeger@billy.biz" },
            new { id = "8", name = "Nicholas Runolfsdottir V", username = "Maxime_Nienow", email = "Sherwood@rosamond.me" },
            new { id = "9", name = "Glenna Reichert", username = "Delphine", email = "Chaim_McDermott@dana.io" },
            new { id = "10", name = "Clementina DuBuque", username = "Moriah.Stanton", email = "Rey.Padberg@karina.biz" }
        };

        return Ok(users);
    }

    [HttpGet("passwords")]
    public async Task<IActionResult> GetAllPasswordRecords(int pg = 1)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // logger.LogWarning(userId);
        var passwordAccounts = await passwordManagerAccountRepository.GetAllPasswordRecordsAsync(Convert.ToInt32(userId));
        return Ok(passwordAccounts);
    }

    [HttpPost("upload-csv")]
    public async Task<IActionResult> UploadCSV(IEnumerable<PasswordAccountDTO> uploadedFileResults)
    {
        if (!uploadedFileResults.Any())
        {
            return BadRequest(new GeneralResponse(Flag: false, Message: "There were no data uploaded"));
        }

        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var response = await passwordManagerAccountRepository.UploadCsvAsync(uploadedFileResults, userId);

        // logger.LogWarning(result.Message);
        // System.Console.WriteLine("nooo");

        if (!response.Flag)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPost("add-passwords")]
    public async Task<IActionResult> AddPasswordRecords([FromBody] string items)
    {
        IEnumerable<PasswordAccountDTO> passwordAccountDTOs = JsonConvert.DeserializeObject<IEnumerable<PasswordAccountDTO>>(items);
        var response = await passwordManagerAccountRepository.CreateMultipleAsync(passwordAccountDTOs);
        if (!response.Flag)
        {
            return BadRequest(new GeneralResponse(Flag: false, Message: response.Message));
        }
        return Ok(new GeneralResponse(Flag: true, Message: response.Message));
    }

    [HttpPost("add-password")]
    public async Task<IActionResult> AddPasswordRecord([FromBody] PasswordAccountDTO passwordAccountDTO)
    {
        var response = await passwordManagerAccountRepository.CreateAsync(passwordAccountDTO);
        if (!response.Flag)
        {
            return BadRequest(response.Message);
        }
        return Ok(response.Message);
    }

    [HttpPut("update-password")]
    public async Task<IActionResult> UpdatePasswordRecord([FromBody] PasswordAccountDTO passwordAccountDTO)
    {
        var response = await passwordManagerAccountRepository.UpdateAsync(passwordAccountDTO);
        if (!response.Flag)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}


