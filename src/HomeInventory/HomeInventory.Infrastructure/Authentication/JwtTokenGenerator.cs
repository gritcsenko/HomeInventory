using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain;
using HomeInventory.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HomeInventory.Infrastructure.Authentication;

internal class JwtTokenGenerator : IAuthenticationTokenGenerator
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IJwtIdentityGenerator _jtiGenerator;
    private readonly JwtSettings _jwtSettings;
    private readonly JwtHeader _header;
    private readonly JwtSecurityTokenHandler _handler = new();

    public JwtTokenGenerator(IDateTimeService dateTimeService, IJwtIdentityGenerator jtiGenerator, IOptions<JwtSettings> jwtOptionsAccessor)
    {
        _dateTimeService = dateTimeService;
        _jtiGenerator = jtiGenerator;
        _jwtSettings = jwtOptionsAccessor.Value;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        _header = new(signingCredentials);
    }

    public async Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        await ValueTask.CompletedTask;

        var securityToken = CreateToken(user);

        return _handler.WriteToken(securityToken);
    }

    private JwtSecurityToken CreateToken(User user)
        => new(_header, CreatePayload(
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, _jtiGenerator.GenerateNew()),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(JwtRegisteredClaimNames.Email, user.Email)));

    private JwtPayload CreatePayload(params Claim[] claims)
    {
        var utcNow = _dateTimeService.Now.UtcDateTime;
        return new(_jwtSettings.Issuer, _jwtSettings.Audience, claims, notBefore: utcNow, expires: utcNow.Add(_jwtSettings.Expiry));
    }
}
