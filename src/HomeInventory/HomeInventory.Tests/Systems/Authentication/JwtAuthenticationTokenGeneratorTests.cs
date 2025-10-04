using System.IdentityModel.Tokens.Jwt;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Domain.UserManagement.ValueObjects;
using HomeInventory.Web.UserManagement.Authentication;
using HomeInventory.Web.UserManagement.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HomeInventory.Tests.Systems.Authentication;

[UnitTest]
public class JwtAuthenticationTokenGeneratorTests : BaseTest
{
    private readonly JwtOptions _options;
    private readonly JwtHeader _expectedHeader;
    private readonly User _user;
    private readonly IJwtIdentityGenerator _jtiGenerator = Substitute.For<IJwtIdentityGenerator>();

    public JwtAuthenticationTokenGeneratorTests()
    {
        Fixture.CustomizeId<UserId>();
        Fixture.CustomizeEmail();
        _options = Fixture.Build<JwtOptions>()
                .With(static x => x.Expiry, TimeSpan.FromSeconds(Fixture.Create<int>()))
                .With(static x => x.Algorithm, SecurityAlgorithms.HmacSha256)
                .Create();
        _expectedHeader = new(_options.CreateSigningCredentials());
        _user = Fixture.Create<User>();
    }

    [Fact]
    public async Task GenerateTokenAsync_Should_GenerateCorrectTokenString()
    {
        var jti = Fixture.Create<string>();
        _jtiGenerator.GenerateNew().Returns(jti);
        var sut = CreateSut();
        var validFrom = DateTime.GetUtcNow().DropSubSeconds().UtcDateTime;

        var actualTokenString = await sut.GenerateTokenAsync(_user, Cancellation.Token);

        using var scope = new AssertionScope();
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

    private JwtAuthenticationTokenGenerator CreateSut() => new(_jtiGenerator, DateTime, Options.Create(_options));
}
