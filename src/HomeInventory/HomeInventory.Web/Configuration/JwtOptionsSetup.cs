using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace HomeInventory.Web.Configuration;

internal class JwtOptionsSetup : IConfigureOptions<JwtOptions>
{
    private const string Section = nameof(JwtOptions);
    private readonly IConfiguration _configuration;

    public JwtOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(JwtOptions options)
    {
        var section = _configuration.GetSection(Section);
        section.Bind(options);
    }
}
