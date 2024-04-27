namespace HomeInventory.Tests.Framework.Customizations;

internal class FromFactoryCustomization<TValue, TObject>(Func<TValue, TObject> createFunc) : ICustomization
{
    private readonly Func<TValue, TObject> _createFunc = createFunc;

    public void Customize(IFixture fixture) => fixture.Customize<TObject>(c => c.FromFactory(_createFunc));
}
