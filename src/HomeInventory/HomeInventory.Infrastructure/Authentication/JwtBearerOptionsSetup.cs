using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HomeInventory.Infrastructure.Authentication;

internal class JwtBearerOptionsSetup : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly JwtOptions _jwtOptions;

    public JwtBearerOptionsSetup(IOptions<JwtOptions> optionsAccessor)
    {
        _jwtOptions = optionsAccessor.Value;
    }

    public void PostConfigure(string name, JwtBearerOptions options)
    {
        if (name == JwtBearerDefaults.AuthenticationScheme)
        {
            options.TokenValidationParameters = CreateTokenValidationParameters();
        }
    }

    private TokenValidationParameters CreateTokenValidationParameters() => new()
    {
        ValidateLifetime = true,

        ValidateIssuer = true,
        ValidIssuers = new[] { _jwtOptions.Issuer },

        ValidateAudience = true,
        ValidAudiences = new[] { _jwtOptions.Audience },

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret)),

        ValidAlgorithms = new[] { _jwtOptions.Algorithm },
    };
}
