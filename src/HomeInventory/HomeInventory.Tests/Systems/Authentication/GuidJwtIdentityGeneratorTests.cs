using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Web.Authentication;

namespace HomeInventory.Tests.Systems.Authentication;

[UnitTest]
public class GuidJwtIdentityGeneratorTests : BaseTest
{
    [Fact]
    public void GenerateNew_Should_ReturnNotEmpty()
    {
        var sut = CreateSut();

        var actual = sut.GenerateNew();

        actual.Should().NotBeEmpty();
    }

    [Fact]
    public void GenerateNew_Should_ReturnNewValue_WhenCalledSecondTime()
    {
        var sut = CreateSut();
        var first = sut.GenerateNew();

        var actual = sut.GenerateNew();

        actual.Should().NotBe(first);
    }

    private static CuidJwtIdentityGenerator CreateSut() => new(IdSuppliers.Ulid);
}
