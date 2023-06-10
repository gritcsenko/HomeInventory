using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Tests.Framework.Customizations;

internal class ApiBehaviorOptionsCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<ApiBehaviorOptions>(c => c
            .Without(x => x.InvalidModelStateResponseFactory));
    }
}
