using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Web.UserManagement.Configuration;
using Microsoft.Extensions.Options;

namespace HomeInventory.Web.UserManagement.Authentication;

internal class JwtAuthenticationTokenGenerator : IAuthenticationTokenGenerator
{
    private readonly IJwtIdentityGenerator _jtiGenerator;
    private readonly TimeProvider _dateTimeService;
    private readonly JwtOptions _jwtOptions;
    private readonly JwtHeader _header;
    private readonly JwtSecurityTokenHandler _handler = new();

    public JwtAuthenticationTokenGenerator(IJwtIdentityGenerator jtiGenerator, TimeProvider dateTimeService, IOptions<JwtOptions> jwtOptionsAccessor)
    {
        _jtiGenerator = jtiGenerator;
        _dateTimeService = dateTimeService;
        _jwtOptions = jwtOptionsAccessor.Value;
        _header = new(_jwtOptions.CreateSigningCredentials());
    }

    public ValueTask<string> GenerateTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        var securityToken = CreateSecurityToken(user);

        var encodedToken = _handler.WriteToken(securityToken);

        return ValueTask.FromResult(encodedToken);
    }

    private JwtSecurityToken CreateSecurityToken(User user)
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
