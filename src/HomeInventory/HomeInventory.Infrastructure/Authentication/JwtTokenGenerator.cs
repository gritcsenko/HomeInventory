using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain;
using HomeInventory.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HomeInventory.Infrastructure.Authentication;

internal class JwtTokenGenerator : IAuthenticationTokenGenerator
{
    private readonly IDateTimeService _dateTimeService;
    private readonly JwtSettings _jwtSettings;
    private readonly JwtHeader _header;

    public JwtTokenGenerator(IDateTimeService dateTimeService, IOptions<JwtSettings> jwtOptionsAccessor)
    {
        _dateTimeService = dateTimeService;
        _jwtSettings = jwtOptionsAccessor.Value;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        _header = new JwtHeader(signingCredentials);
    }

    public async Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        await ValueTask.CompletedTask;

        var securityToken = new JwtSecurityToken(_header, CreatePayload(
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)));

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    private JwtPayload CreatePayload(params Claim[] claims)
    {
        var utcNow = _dateTimeService.Now.UtcDateTime;
        return new JwtPayload(_jwtSettings.Issuer, _jwtSettings.Audience, claims, utcNow, utcNow.Add(_jwtSettings.Expiry));
    }
}
