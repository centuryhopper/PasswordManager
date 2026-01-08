using System.Drawing;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using Server.Repositories;
using Server.Utils;
using Shared.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountRepository accountRepository, ILogger<AccountController> logger, IWebHostEnvironment env, IConfiguration configuration) : ControllerBase
{
    [HttpGet("nlog-test")]
    public IActionResult Test()
    {
        logger.LogWarning("warning!");
        logger.LogInformation("information!");
        logger.LogError("error!");
        logger.LogCritical("critical");
        logger.LogDebug("debug");
        return Ok("Logging test completed. Check your PostgreSQL LOGS table.");
    }

    [Authorize]
    [HttpPost("logout/{userId:int}")]
    public async Task<IActionResult> Logout(int userId)
    {
        var response = await accountRepository.Logout(userId);
        if (!response.Flag)
        {
            return BadRequest(response);
        }
        //logger.LogInformation(response.Message);
        return Ok(response);
    }

    [HttpGet("check-user/{email}/{password}")]
    public async Task<IActionResult> CheckPasswordManagerUser(string email, string password)
    {
        var response = await accountRepository.CheckPasswordManagerUser(new LoginDTO { Email = email, Password = password });
        return Ok(response);
    }

    [HttpGet("login-with-2fa")]
    public async Task<IActionResult> VerifyTwoFactorCode([FromQuery] string email, [FromQuery] string code)
    {
        var response = await accountRepository.LoginAccountWith2FA(email, code);
        if (!response.Flag)
        {
            return BadRequest(response);
        }
        //logger.LogInformation(response.Message);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("get-2fa-status/{email}")]
    public async Task<IActionResult> GetTwoFactorStatus(string email)
    {
        var twoFactorStatus = await accountRepository.GetTwoFactorStatus(email);
        return Ok(new { twoFactorStatus });
    }

    [HttpPost("send-2fa-code/{email}")]
    public async Task<IActionResult> SendTwoFactorCode(string email)
    {
        var (msg, flag) = await accountRepository.SendTwoFactorCode(email);
        //System.Console.WriteLine(msg);
        if (!flag)
        {
            return BadRequest(new GeneralResponse(false, msg));
        }

        var smtpInfo = env.IsDevelopment() ? configuration.GetConnectionString("smtp_client").Split("|") : Environment.GetEnvironmentVariable("smtp_client").Split("|");

        await Helpers.SendEmailAsync(
            subject: "2FA Verification",
            senderEmail: smtpInfo[0],
            senderPassword: smtpInfo[1],
            body: Helpers.Build2FAHtmlEmail(email: email, twoFaToken: msg),
            receivers: [email!],
            textFormat: TextFormat.Html
        );
        
        return Ok(new GeneralResponse(true, msg));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO loginDTO)
    {
        var response = await accountRepository.LoginAccount(loginDTO);
        if (!response.Flag)
        {
            return BadRequest(response);
        }
        //logger.LogInformation(response.Message);
        return Ok(response);
    }
}
