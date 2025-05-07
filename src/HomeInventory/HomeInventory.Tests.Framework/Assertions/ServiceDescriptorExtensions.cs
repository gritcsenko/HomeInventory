using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Framework.Assertions;

internal static class ServiceDescriptorExtensions
{
    public static object? GetInstance(this ServiceDescriptor descriptor, IServiceProvider provider) =>
        descriptor switch
        {
            { ImplementationType: { } type } => ActivatorUtilities.CreateInstance(provider, type),
            { ImplementationFactory: { } factory } => factory(provider),
            _ => descriptor.ImplementationInstance,
        };
}
