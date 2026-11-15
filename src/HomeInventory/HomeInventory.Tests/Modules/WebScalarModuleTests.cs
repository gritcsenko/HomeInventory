using System.Diagnostics.CodeAnalysis;
using HomeInventory.Web.OpenApi;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace HomeInventory.Tests.Modules;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class WebScalarModuleTests() : BaseModuleTest<WebScalarModule>(static () => new())
{
    protected override void EnsureRegistered(IServiceCollection services) =>
        services.Should()
            .Contain(d => d.ServiceType == typeof(IApiDescriptionGroupCollectionProvider));
}

