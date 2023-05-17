using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Primitives;
using HomeInventory.Web.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HomeInventory.Web.Authentication;

internal class JwtTokenGenerator : IAuthenticationTokenGenerator
{
    private readonly IJwtIdentityGenerator _jtiGenerator;
    private readonly JwtOptions _jwtOptions;
    private readonly JwtHeader _header;
    private readonly JwtSecurityTokenHandler _handler = new();

    public JwtTokenGenerator(IJwtIdentityGenerator jtiGenerator, IOptions<JwtOptions> jwtOptionsAccessor)
    {
        _jtiGenerator = jtiGenerator;
        _jwtOptions = jwtOptionsAccessor.Value;

        var signingCredentials = new SigningCredentials(_jwtOptions.SecurityKey, _jwtOptions.Algorithm);
        _header = new(signingCredentials);
    }

    public async Task<string> GenerateTokenAsync(User user, IDateTimeService dateTimeService, CancellationToken cancellationToken = default)
    {
        await ValueTask.CompletedTask;

        var securityToken = CreateToken(dateTimeService, user);

        return _handler.WriteToken(securityToken);
    }

    private JwtSecurityToken CreateToken(IDateTimeService dateTimeService, User user)
        => new(_header, CreatePayload(
            dateTimeService,
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, _jtiGenerator.GenerateNew()),
            new(JwtRegisteredClaimNames.Email, user.Email.Value, ClaimValueTypes.Email)));

    private JwtPayload CreatePayload(IDateTimeService dateTimeService, params Claim[] claims)
    {
        var utcNow = dateTimeService.UtcNow.UtcDateTime;
        return new(_jwtOptions.Issuer, _jwtOptions.Audience, claims, notBefore: utcNow, expires: utcNow.Add(_jwtOptions.Expiry), issuedAt: utcNow);
    }
}
