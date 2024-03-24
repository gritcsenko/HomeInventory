using HomeInventory.Infrastructure.Services;

namespace HomeInventory.Tests.Systems.Authentication;

[UnitTest]
public class BCryptPasswordHasherTests() : BaseTest<BCryptPasswordHasherTests.GivenContext>(t => new(t))
{
    private static readonly Variable<string> _password = new(nameof(_password));

    [Fact]
    public async Task HashAsync_ShouldReturnSomethingDifferentFromInput()
    {
        Given
            .New(_password)
            .AddSut();

        var then = await When
            .InvokedAsync(Given.Sut, _password, async (sut, password, ct) => await sut.HashAsync(password, ct));

        then
            .Result(_password, (actual, password) =>
                actual.Should().NotBe(password));
    }

    [Fact]
    public async Task HashAsync_ShouldReturnDifferentHashesForDifferentInputs()
    {
        Given
            .New(_password, 2)
            .AddSut();

        var then = await When
            .InvokedAsync(Given.Sut, _password[0], _password[1], async (sut, password1, password2, ct) =>
            {
                return new[] { await sut.HashAsync(password1, ct), await sut.HashAsync(password2, ct) };
            });

        then
            .Result(actual =>
                actual[0].Should().NotBe(actual[1]));
    }

    [Fact]
    public async Task VerifyAsync_ShouldConfirmHashed()
    {
        Given
            .New(_password)
            .AddSut();

        var then = await When
            .InvokedAsync(Given.Sut, _password, async (sut, password, ct) =>
            {
                var hash = await sut.HashAsync(password, ct);
                return await sut.VarifyHashAsync(password, hash, ct);
            });

        then
            .Result(actual =>
                actual.Should().BeTrue());
    }

#pragma warning disable CA1034 // Nested types should not be visible
    public sealed class GivenContext(BaseTest test) : GivenContext<GivenContext>(test)
#pragma warning restore CA1034 // Nested types should not be visible
    {
        private readonly Variable<BCryptPasswordHasher> _sut = new(nameof(_sut));

        internal IVariable<BCryptPasswordHasher> Sut => _sut;

        internal GivenContext AddSut() => Add(_sut, CreateSut);

        private static BCryptPasswordHasher CreateSut() => new() { WorkFactor = 6 };
    }
}
