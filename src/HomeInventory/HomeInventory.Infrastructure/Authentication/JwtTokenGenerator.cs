using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Services;
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

    public JwtTokenGenerator(IDateTimeService dateTimeService, IOptions<JwtSettings> jwtOptionsAccessor)
    {
        _dateTimeService = dateTimeService;
        _jwtSettings = jwtOptionsAccessor.Value;
    }

    public async Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        await ValueTask.CompletedTask;

        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)), SecurityAlgorithms.HmacSha256);
        var header = new JwtHeader(signingCredentials);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };
        var utcNow = _dateTimeService.Now.UtcDateTime;
        var payload = new JwtPayload(_jwtSettings.Issuer, _jwtSettings.Audience, claims, null, utcNow.Add(_jwtSettings.Expiry));

        var securityToken = new JwtSecurityToken(header, payload);
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}
