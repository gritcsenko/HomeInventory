﻿using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class AmountValueObjectConverterTests : IAsyncLifetime
{
    private readonly InternalAmountValueObjectConverterTests _test = new();

    public Task DisposeAsync() => _test.DisposeAsync();

    public Task InitializeAsync() => _test.InitializeAsync();

    [Fact]
    public void TryConvert() => _test.TryConvert();
}

internal class InternalAmountValueObjectConverterTests() : BaseTest<AmountValueObjectConverterTestsGivenContext>(static t => new(t))
{
    public void TryConvert()
    {
        Given
            .New<Amount>(out var amount)
            .New<ProductAmountModel>(out var amountModel)
            .SubstituteFor(out IVariable<IAmountFactory> factory, amount,
                (factory, amount) => factory
                    .Create(Arg.Any<decimal>(), Arg.Any<AmountUnit>())
                    .Returns(amount))
            .Sut(out var sut, factory);

        When
            .Invoked(sut, amountModel, (sut, amount) => sut.TryConvert(amount))
            .Result(amount, (r, a) => r.Should().BeSuccess(x => x.Should().BeSameAs(a)));
    }
}

internal sealed class AmountValueObjectConverterTestsGivenContext : GivenContext<AmountValueObjectConverterTestsGivenContext, AmountObjectConverter, IAmountFactory>
{
    public AmountValueObjectConverterTestsGivenContext(BaseTest test)
        : base(test)
    {
        Customize(new AmountUnitCustomization());
        Customize(new ProductAmountModelCustomization());
        Customize(new AmountCustomization());
    }

    protected override AmountObjectConverter CreateSut(IAmountFactory factory) => new(factory);

    private sealed class ProductAmountModelCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<ProductAmountModel>(static c => c
                .With<string, AmountUnit>(static m => m.UnitName, static u => u.Name));
        }
    }
    private sealed class AmountUnitCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var rnd = new Random();
            var items = EnumerationItemsCollection.CreateFor<AmountUnit>();
            fixture.Customize<AmountUnit>(c => c.FromFactory(() => (AmountUnit)rnd.Peek(items)));
        }
    }

    private sealed class AmountCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<Amount>(static c => c.FromFactory<decimal, AmountUnit>(static (v, u) => new Amount(v, u)));
        }
    }
}
