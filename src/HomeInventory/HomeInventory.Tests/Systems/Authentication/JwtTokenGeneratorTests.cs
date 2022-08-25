using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AutoFixture;
using FluentAssertions;
using HomeInventory.Domain;
using HomeInventory.Domain.Entities;
using HomeInventory.Infrastructure.Authentication;
using HomeInventory.Tests.Customizations;
using HomeInventory.Tests.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;

namespace HomeInventory.Tests.Systems.Authentication;

[Trait("Category", "Unit")]
public class JwtTokenGeneratorTests : BaseTest
{
    private readonly IDateTimeService _dateTimeService;
    private readonly JwtSettings _settings;
    private readonly User _user;
    private readonly IJwtIdentityGenerator _jtiGenerator;

    public JwtTokenGeneratorTests()
    {
        Fixture.Customize(new UserIdCustomization());
        _dateTimeService = Substitute.For<IDateTimeService>();
        _settings = Fixture.Build<JwtSettings>()
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
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var expectedHeader = new JwtHeader(new SigningCredentials(key, _settings.Algorithm));
        var jti = Fixture.Create<string>();
        _jtiGenerator.GenerateNew().Returns(jti);
        var now = DateTimeOffset.Now;
        _dateTimeService.Now.Returns(now);
        var validFrom = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Offset).UtcDateTime;

        var actualTokenString = await sut.GenerateTokenAsync(_user, CancellationToken);

        actualTokenString.Should().NotBeNullOrEmpty();
        var actualToken = new JwtSecurityTokenHandler().ReadJwtToken(actualTokenString);
        actualToken.Header.Should().BeEquivalentTo(expectedHeader);
        actualToken.Issuer.Should().Be(_settings.Issuer);
        actualToken.Audiences.Should().Contain(_settings.Audience)
            .And.HaveCount(1);
        actualToken.ValidFrom.Should().Be(validFrom);
        actualToken.ValidTo.Should().Be(validFrom.Add(_settings.Expiry));
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
            .Which.Should().Be(_user.Email);
    }

    private JwtTokenGenerator CreateSut() => new(_dateTimeService, _jtiGenerator, Options.Create(_settings));
}
