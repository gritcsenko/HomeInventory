using HomeInventory.Web.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace HomeInventory.Tests.Systems.Authentication;

[UnitTest]
public class JwtBearerOptionsSetupTests : BaseTest
{
    [Fact]
    public void Configure_Should_SetupOptions()
    {
        var jwtOptions = Fixture.Create<JwtOptions>();
        var sut = new JwtBearerOptionsSetup(Options.Create(jwtOptions));
        var bearerOptions = new JwtBearerOptions();

        sut.Configure(bearerOptions);

        var parameters = bearerOptions.TokenValidationParameters;
        parameters.Should().NotBeNull();
        parameters.ValidateLifetime.Should().BeTrue();
        parameters.ValidateIssuer.Should().BeTrue();
        parameters.ValidIssuers.Should().BeEquivalentTo(new[] { jwtOptions.Issuer });
        parameters.ValidateAudience.Should().BeTrue();
        parameters.ValidAudiences.Should().BeEquivalentTo(new[] { jwtOptions.Audience });
        parameters.ValidateIssuerSigningKey.Should().BeTrue();
        parameters.IssuerSigningKey.Should().Be(jwtOptions.SecurityKey);
        parameters.ValidAlgorithms.Should().BeEquivalentTo(new[] { jwtOptions.Algorithm });
    }
}
