using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Infrastructure.Persistence.Models;
using OneOf;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class AmountValueObjectConverterTests() : BaseTest<AmountValueObjectConverterTests.GivenTestContext>(t => new(t))
{
    private static readonly Variable<AmountObjectConverter> _sut = new(nameof(_sut));
    private static readonly Variable<IAmountFactory> _factory = new(nameof(_factory));
    private static readonly Variable<ProductAmountModel> _amountModel = new(nameof(_amountModel));
    private static readonly Variable<Amount> _amount = new(nameof(_amount));

    [Fact]
    public void Should()
    {
        Given
            .New(_amount)
            .New(_amountModel)
            .SubstituteFor(_factory, _amount,
                (factory, amount) => factory
                    .Create(Arg.Any<decimal>(), Arg.Any<AmountUnit>())
                    .Returns(OneOf<Amount, IError>.FromT0(amount)))
            .Sut(_sut, _factory);

        When
            .Invoked(_sut, _amountModel, (sut, amount) => sut.TryConvert(amount))
            .Result(_amount, (r, a) =>
            {
                r.IsT0.Should().BeTrue();
                r.AsT0.Should().BeSameAs(a);
            });
    }

#pragma warning disable CA1034 // Nested types should not be visible
    public sealed class GivenTestContext : GivenContext<GivenTestContext>
#pragma warning restore CA1034 // Nested types should not be visible
    {
        public GivenTestContext(BaseTest test)
            : base(test)
        {
            Customize(new AmountUnitCustomization());
            Customize(new ProductAmountModelCustomization());
            Customize(new AmountCustomization());
        }

        internal GivenTestContext Sut(IVariable<AmountObjectConverter> sut, IVariable<IAmountFactory> factoryVariable)
        {
            var factory = GetValue(factoryVariable);
            Add(sut, () => new(factory));
            return this;
        }

        private sealed class ProductAmountModelCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<ProductAmountModel>(c => c
                    .With<string, AmountUnit>(m => m.UnitName, u => u.Name));
            }
        }
        private sealed class AmountUnitCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                var rnd = new Random();
                var items = EnumerationItemsCollection.CreateFor<AmountUnit>();
                fixture.Customize<AmountUnit>(c => c.FromFactory(() => rnd.Peek(items).Value));
            }
        }

        private sealed class AmountCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<Amount>(c => c.FromFactory<decimal, AmountUnit>((v, u) => new Amount(v, u)));
            }
        }
    }
}
