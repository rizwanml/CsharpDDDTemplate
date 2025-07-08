using System.IdentityModel.Tokens.Jwt;

namespace CqrsService.Shared;

public static class JwtHandler
{
    public static JwtSecurityToken ReadJwt(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwtToken);
        return token;
    }
}