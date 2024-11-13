using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Infrastructure.Services;

namespace HomeInventory.Tests.Systems.Authentication;

[UnitTest]
public class BCryptPasswordHasherTests() : BaseTest<BCryptPasswordHasherTestsGivenContext>(static t => new(t))
{
    [Fact]
    public async Task HashAsync_ShouldReturnSomethingDifferentFromInput()
    {
        Given
            .New<string>(out var password)
            .Sut(out var sut);

        var then = await When
            .InvokedAsync(sut, password, static async (sut, password, ct) => await sut.HashAsync(password, ct));

        then
            .Result(password, static (actual, password) =>
                actual.Should().NotBe(password));
    }

    [Fact]
    public async Task HashAsync_ShouldReturnDifferentHashesForDifferentInputs()
    {
        Given
            .New<string>(out var password, 2)
            .Sut(out var sut);

        var then = await When
            .InvokedAsync(sut, password[0], password[1], static async (sut, password1, password2, ct) =>
            {
                return new[] { await sut.HashAsync(password1, ct), await sut.HashAsync(password2, ct) };
            });

        then
            .Result(static actual =>
                actual[0].Should().NotBe(actual[1]));
    }

    [Fact]
    public async Task VerifyAsync_ShouldConfirmHashed()
    {
        Given
            .New<string>(out var password)
            .Sut(out var sut);

        var then = await When
            .InvokedAsync(sut, password, static async (sut, password, ct) =>
            {
                var hash = await sut.HashAsync(password, ct);
                return await sut.VarifyHashAsync(password, hash, ct);
            });

        then
            .Result(static actual =>
                actual.Should().BeTrue());
    }
}

public sealed class BCryptPasswordHasherTestsGivenContext(BaseTest test) : GivenContext<BCryptPasswordHasherTestsGivenContext, IPasswordHasher>(test)
{
    protected override IPasswordHasher CreateSut() => new BCryptPasswordHasher() { WorkFactor = 6 };
}
