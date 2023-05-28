using System.IdentityModel.Tokens.Jwt;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Web.Authentication;
using HomeInventory.Web.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HomeInventory.Tests.Systems.Authentication;

[UnitTest]
public class JwtTokenGeneratorTests : BaseTest
{
    private readonly JwtOptions _options;
    private readonly JwtHeader _expectedHeader;
    private readonly User _user;
    private readonly IJwtIdentityGenerator _jtiGenerator = Substitute.For<IJwtIdentityGenerator>();

    public JwtTokenGeneratorTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();
        _options = Fixture.Build<JwtOptions>()
                .With(x => x.Expiry, TimeSpan.FromSeconds(Fixture.Create<int>()))
                .With(x => x.Algorithm, SecurityAlgorithms.HmacSha256)
                .Create();
        _expectedHeader = new JwtHeader(new SigningCredentials(_options.SecurityKey, _options.Algorithm));
        _user = Fixture.Create<User>();
    }

    [Fact]
    public async Task GenerateTokenAsync_Should_GenerateCorrectTokenString()
    {
        var jti = Fixture.Create<string>();
        _jtiGenerator.GenerateNew().Returns(jti);
        var sut = CreateSut();
        var validFrom = DateTime.UtcNow.DropSubSeconds().UtcDateTime;

        var actualTokenString = await sut.GenerateTokenAsync(_user, Cancellation.Token);

        actualTokenString.Should().NotBeNullOrEmpty();
        var actualToken = new JwtSecurityTokenHandler().ReadJwtToken(actualTokenString);
        actualToken.Header.Should().BeEquivalentTo(_expectedHeader);
        actualToken.Issuer.Should().Be(_options.Issuer);
        actualToken.Audiences.Should().OnlyContain(a => a == _options.Audience)
            .And.ContainSingle();
        actualToken.ValidFrom.Should().Be(validFrom);
        actualToken.ValidTo.Should().Be(validFrom.Add(_options.Expiry));
        actualToken.Payload.Should().Contain(JwtRegisteredClaimNames.Sub, _user.Id.ToString());
        actualToken.Payload.Should().Contain(JwtRegisteredClaimNames.Jti, jti);
        actualToken.Payload.Should().Contain(JwtRegisteredClaimNames.Email, _user.Email.Value);
    }

    private JwtTokenGenerator CreateSut() => new(_jtiGenerator, DateTime, Options.Create(_options));
}
