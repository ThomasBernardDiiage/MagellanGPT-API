using System.IdentityModel.Tokens.Jwt;
using Magellan.Api.Services.Interfaces;

namespace Magellan.Api.Services;

public class TokenService : ITokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCurrentToken()
    {
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException("JWT Token is missing");
        }

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
        {
            throw new UnauthorizedAccessException("Invalid JWT Token");
        }

        var subClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub");

        if (subClaim == null)
        {
            throw new InvalidOperationException("sub claim not found");
        }

        return subClaim.Value;
    }

}