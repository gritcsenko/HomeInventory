namespace HomeInventory.Tests.Systems.Authentication;

[UnitTest]
public class BCryptPasswordHasherTests : BaseTest<BCryptPasswordHasherTestsGivenContext>
{
    private static readonly Variable<string> _password = new(nameof(_password));

    [Fact]
    public async Task HashAsync_ShouldReturnSomethingDifferentFromInput()
    {
        Given
            .New(_password)
            .Hasher();

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
            .Hasher();

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
            .Hasher();

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

    protected override BCryptPasswordHasherTestsGivenContext CreateGiven(VariablesContainer variables) => new(variables, Fixture);
}
