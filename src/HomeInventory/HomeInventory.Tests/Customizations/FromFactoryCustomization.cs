using AutoFixture;

namespace HomeInventory.Tests.Customizations;

internal class FromFactoryCustomization<TValue, TObject> : ICustomization
{
    private readonly Func<TValue, TObject> _createFunc;

    public FromFactoryCustomization(Func<TValue, TObject> createFunc)
    {
        _createFunc = createFunc;
    }

    public void Customize(IFixture fixture) => fixture.Customize<TObject>(c => c.FromFactory(_createFunc));
}
