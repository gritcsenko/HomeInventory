using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Helpers;
internal class TestingServiceCollection : List<ServiceDescriptor>, IServiceCollection
{
}
