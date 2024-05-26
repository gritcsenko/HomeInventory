using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Web.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HomeInventory.Web.Authentication;

internal class JwtTokenGenerator : IAuthenticationTokenGenerator
{
    private readonly IJwtIdentityGenerator _jtiGenerator;
    private readonly TimeProvider _dateTimeService;
    private readonly JwtOptions _jwtOptions;
    private readonly JwtHeader _header;
    private readonly JwtSecurityTokenHandler _handler = new();

    public JwtTokenGenerator(IJwtIdentityGenerator jtiGenerator, TimeProvider dateTimeService, IOptions<JwtOptions> jwtOptionsAccessor)
    {
        _jtiGenerator = jtiGenerator;
        _dateTimeService = dateTimeService;
        _jwtOptions = jwtOptionsAccessor.Value;

        var signingCredentials = new SigningCredentials(_jwtOptions.SecurityKey, _jwtOptions.Algorithm);
        _header = new(signingCredentials);
    }

    public ValueTask<string> GenerateTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        var securityToken = CreateToken(user);

        var token = _handler.WriteToken(securityToken);

        return ValueTask.FromResult(token);
    }

    private JwtSecurityToken CreateToken(User user)
        => new(_header, CreatePayload(
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, _jtiGenerator.GenerateNew()),
            new(JwtRegisteredClaimNames.Email, user.Email.Value, ClaimValueTypes.Email)));

    private JwtPayload CreatePayload(params Claim[] claims)
    {
        var utcNow = _dateTimeService.GetUtcNow().UtcDateTime;
        return new(_jwtOptions.Issuer, _jwtOptions.Audience, claims, notBefore: utcNow, expires: utcNow.Add(_jwtOptions.Expiry), issuedAt: utcNow);
    }
}
