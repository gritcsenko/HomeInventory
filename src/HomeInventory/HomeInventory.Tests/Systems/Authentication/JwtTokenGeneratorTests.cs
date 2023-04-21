using System.IdentityModel.Tokens.Jwt;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Web.Authentication;
using HomeInventory.Web.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HomeInventory.Tests.Systems.Authentication;

[Trait("Category", "Unit")]
public class JwtTokenGeneratorTests : BaseTest
{
    private readonly IDateTimeService _dateTimeService = Substitute.For<IDateTimeService>();
    private readonly JwtOptions _options;
    private readonly User _user;
    private readonly IJwtIdentityGenerator _jtiGenerator;

    public JwtTokenGeneratorTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();
        _options = Fixture.Build<JwtOptions>()
                .With(x => x.Expiry, TimeSpan.FromSeconds(Fixture.Create<int>()))
                .With(x => x.Algorithm, SecurityAlgorithms.HmacSha256)
                .Create();
        _user = Fixture.Create<User>();
        _jtiGenerator = Substitute.For<IJwtIdentityGenerator>();
    }

    [Fact]
    public async Task GenerateTokenAsync_Should_GenerateCorrectTokenString()
    {
        var sut = CreateSut();
        var expectedHeader = new JwtHeader(new SigningCredentials(_options.SecurityKey, _options.Algorithm));
        var jti = Fixture.Create<string>();
        _jtiGenerator.GenerateNew().Returns(jti);
        var now = DateTimeOffset.Now;
        _dateTimeService.Now.Returns(now);
        var validFrom = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Offset).UtcDateTime;

        var actualTokenString = await sut.GenerateTokenAsync(_user, CancellationToken);

        actualTokenString.Should().NotBeNullOrEmpty();
        var actualToken = new JwtSecurityTokenHandler().ReadJwtToken(actualTokenString);
        actualToken.Header.Should().BeEquivalentTo(expectedHeader);
        actualToken.Issuer.Should().Be(_options.Issuer);
        actualToken.Audiences.Should().Contain(_options.Audience)
            .And.HaveCount(1);
        actualToken.ValidFrom.Should().Be(validFrom);
        actualToken.ValidTo.Should().Be(validFrom.Add(_options.Expiry));
        actualToken.Payload.Should().ContainKey(JwtRegisteredClaimNames.Sub)
            .WhoseValue.Should().BeOfType<string>()
            .Which.Should().Be(_user.Id.ToString());
        actualToken.Payload.Should().ContainKey(JwtRegisteredClaimNames.Jti)
            .WhoseValue.Should().BeOfType<string>()
            .Which.Should().Be(jti);
        actualToken.Payload.Should().ContainKey(JwtRegisteredClaimNames.Email)
            .WhoseValue.Should().BeOfType<string>()
            .Which.Should().Be(_user.Email.Value);
    }

    private JwtTokenGenerator CreateSut() => new(_dateTimeService, _jtiGenerator, Options.Create(_options));
}
