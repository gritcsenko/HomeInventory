using HomeInventory.Infrastructure.Services;

namespace HomeInventory.Tests.Systems.Authentication;

public sealed class BCryptPasswordHasherTestsGivenContext : GivenContext<BCryptPasswordHasherTestsGivenContext>
{
    private readonly Variable<BCryptPasswordHasher> _sut = new(nameof(_sut));

    public BCryptPasswordHasherTestsGivenContext(VariablesContainer variables, IFixture fixture)
        : base(variables, fixture)
    {
    }

    internal IIndexedVariable<BCryptPasswordHasher> Sut => _sut.WithIndex(0);

    internal BCryptPasswordHasherTestsGivenContext Hasher()
    {
        Add(_sut, () => new BCryptPasswordHasher() { WorkFactor = 6 });
        return this;
    }
}
