
namespace Shared;

public static class JwtConfig
{
    public static readonly string JWT_TOKEN_NAME = "authToken";
    public static readonly DateTime JWT_TOKEN_EXP_DATETIME = DateTime.Now.AddDays(1);
    public static readonly long JWT_TOKEN_EXP_DATE = new DateTimeOffset(JWT_TOKEN_EXP_DATETIME).ToUnixTimeMilliseconds();
}

