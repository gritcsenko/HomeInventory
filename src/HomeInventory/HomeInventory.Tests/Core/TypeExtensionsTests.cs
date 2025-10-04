namespace HomeInventory.Tests.Core;

[UnitTest]
public class TypeExtensionsTests : BaseTest
{
    [Fact]
    public void GetFormattedName_Should_NotFormatNonGeneric()
    {
        var type = typeof(NonGenericClass);

        var actual = type.GetFormattedName();

        actual.Should().Be(type.Name);
    }

    [Fact]
    public void GetFormattedName_Should_FormatGeneric()
    {
        var type = typeof(GenericClass<string>);

        var actual = type.GetFormattedName();

        actual.Should().Be($"GenericClass<String>");
    }

    [Fact]
    public void GetFormattedName_Should_FormatGeneric2()
    {
        var type = typeof(GenericClass<string, int>);

        var actual = type.GetFormattedName();

        actual.Should().Be($"GenericClass<String,Int32>");
    }

    [Fact]
    public void GetFieldsOfType_Should_ReturnValues()
    {
        E.f1 = Fixture.Create<E>();
        E.f2 = Fixture.Create<E>();
        E._f3 = Fixture.Create<E>();
        var type = typeof(E);

        var actual = type.GetFieldValuesOfType<E>().ToArray();

        using var scope = new AssertionScope();
        actual.Should().HaveCount(2);
        actual.Should().Contain(E.f1);
        actual.Should().Contain(E.f2);
        actual.Should().NotContain(E._f3);
    }

    private class GenericClass<T1, T2> : Dictionary<T1, T2>
        where T1 : notnull
    {
    }

    private class GenericClass<T> : GenericClass<T, object>
        where T : notnull
    {
    }

#pragma warning disable S2094
    private class NonGenericClass
    {
    }
#pragma warning restore S2094

    private class E
    {
        public static E f1 = null!;
        public static E f2 = null!;
        internal static E _f3 = null!;
    }
}
