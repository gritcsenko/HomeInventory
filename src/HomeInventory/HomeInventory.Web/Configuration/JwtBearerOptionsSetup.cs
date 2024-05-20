using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HomeInventory.Web.Configuration;

internal class JwtBearerOptionsSetup(IOptions<JwtOptions> optionsAccessor) : IConfigureOptions<JwtBearerOptions>
{
    private readonly JwtOptions _jwtOptions = optionsAccessor.Value;

    public void Configure(JwtBearerOptions options) => options.TokenValidationParameters = CreateTokenValidationParameters();

    private TokenValidationParameters CreateTokenValidationParameters() => new()
    {
        ValidateLifetime = true,

        ValidateIssuer = true,
        ValidIssuers = [_jwtOptions.Issuer],

        ValidateAudience = true,
        ValidAudiences = [_jwtOptions.Audience],

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = _jwtOptions.SecurityKey,

        ValidAlgorithms = [_jwtOptions.Algorithm],

        ClockSkew = _jwtOptions.ClockSkew,
    };
}
