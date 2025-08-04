
namespace Shared.Models;

public static class JwtConfig
{
    public static readonly string JWT_TOKEN_NAME = "JWT_TOKEN_NAME";
    public static readonly string JWT_TOKEN_EXP_DATE_NAME = "JWT_TOKEN_EXP_DATE_NAME";
    public static DateTime JWT_TOKEN_EXP_DATETIME => DateTime.UtcNow.AddDays(30);
}

