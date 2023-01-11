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
    private readonly IDateTimeService _dateTimeService;
    private readonly IJwtIdentityGenerator _jtiGenerator;
    private readonly JwtOptions _jwtOptions;
    private readonly JwtHeader _header;
    private readonly JwtSecurityTokenHandler _handler = new();

    public JwtTokenGenerator(IDateTimeService dateTimeService, IJwtIdentityGenerator jtiGenerator, IOptions<JwtOptions> jwtOptionsAccessor)
    {
        _dateTimeService = dateTimeService;
        _jtiGenerator = jtiGenerator;
        _jwtOptions = jwtOptionsAccessor.Value;

        var signingCredentials = new SigningCredentials(_jwtOptions.SecurityKey, _jwtOptions.Algorithm);
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
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, _jtiGenerator.GenerateNew()),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(JwtRegisteredClaimNames.Email, user.Email.Value, ClaimValueTypes.Email)));

    private JwtPayload CreatePayload(params Claim[] claims)
    {
        var utcNow = _dateTimeService.Now.UtcDateTime;
        return new(_jwtOptions.Issuer, _jwtOptions.Audience, claims, notBefore: utcNow, expires: utcNow.Add(_jwtOptions.Expiry), issuedAt: utcNow);
    }
}
