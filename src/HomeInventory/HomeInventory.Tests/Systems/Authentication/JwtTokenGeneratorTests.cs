using System.IdentityModel.Tokens.Jwt;
using AutoFixture;
using FluentAssertions;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Primitives;
using HomeInventory.Tests.Customizations;
using HomeInventory.Tests.Helpers;
using HomeInventory.Web.Authentication;
using HomeInventory.Web.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;

namespace HomeInventory.Tests.Systems.Authentication;

[Trait("Category", "Unit")]
public class JwtTokenGeneratorTests : BaseTest
{
    private readonly IDateTimeService _dateTimeService;
    private readonly JwtOptions _options;
    private readonly User _user;
    private readonly IJwtIdentityGenerator _jtiGenerator;

    public JwtTokenGeneratorTests()
    {
        Fixture.Customize(new UserIdCustomization());
        Fixture.Customize(new EmailCustomization());
        _dateTimeService = Substitute.For<IDateTimeService>();
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
        var key = new SymmetricSecurityKey(_options.Key);
        var expectedHeader = new JwtHeader(new SigningCredentials(key, _options.Algorithm));
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
        actualToken.Payload.Should().ContainKey(JwtRegisteredClaimNames.GivenName)
            .WhoseValue.Should().BeOfType<string>()
            .Which.Should().Be(_user.FirstName);
        actualToken.Payload.Should().ContainKey(JwtRegisteredClaimNames.FamilyName)
            .WhoseValue.Should().BeOfType<string>()
            .Which.Should().Be(_user.LastName);
        actualToken.Payload.Should().ContainKey(JwtRegisteredClaimNames.Email)
            .WhoseValue.Should().BeOfType<string>()
            .Which.Should().Be(_user.Email.Value);
    }

    private JwtTokenGenerator CreateSut() => new(_dateTimeService, _jtiGenerator, Options.Create(_options));
}
