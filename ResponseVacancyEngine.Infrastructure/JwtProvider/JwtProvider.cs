using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ResponseVacancyEngine.Application.Infrastructure.Interfaces.JwtProvider;
using ResponseVacancyEngine.Infrastructure.Options;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Infrastructure.JwtProvider;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    public string GenerateToken(Account account)
    {
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, account.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(_options.ExpiresHours)
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenValue;
    }

}