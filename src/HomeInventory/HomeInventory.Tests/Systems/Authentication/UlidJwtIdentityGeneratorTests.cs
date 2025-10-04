using HomeInventory.Web.UserManagement.Authentication;

namespace HomeInventory.Tests.Systems.Authentication;

[UnitTest]
public class UlidJwtIdentityGeneratorTests : BaseTest
{
    [Fact]
    public void GenerateNew_Should_ReturnNotEmpty()
    {
        var sut = new UlidJwtIdentityGenerator();

        var actual = sut.GenerateNew();

        actual.Should().NotBeEmpty();
    }

    [Fact]
    public void GenerateNew_Should_ReturnNewValue_WhenCalledSecondTime()
    {
        var sut = new UlidJwtIdentityGenerator();
        var first = sut.GenerateNew();

        var actual = sut.GenerateNew();

        actual.Should().NotBe(first);
    }
}
