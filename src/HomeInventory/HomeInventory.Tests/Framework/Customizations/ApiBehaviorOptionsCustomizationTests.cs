using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Tests.Framework.Customizations;

[UnitTest]
public class ApiBehaviorOptionsCustomizationTests : BaseTest
{
    [Fact]
    public void Customize_Should_ProvideCorrectCustomization()
    {
        Fixture.Customize(new ApiBehaviorOptionsCustomization());
        var sut = Fixture.Create<ApiBehaviorOptions>();

        var actual = sut.InvalidModelStateResponseFactory;

        actual.Should().BeNull();
    }
}
