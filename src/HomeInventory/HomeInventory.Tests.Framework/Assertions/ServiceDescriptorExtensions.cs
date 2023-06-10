using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Framework.Assertions;

internal static class ServiceDescriptorExtensions
{
    public static object? GetInstance(this ServiceDescriptor descriptor, IServiceProvider provider)
    {
        if (descriptor.ImplementationType is { } type)
        {
            return ActivatorUtilities.CreateInstance(provider, type);
        }
        if (descriptor.ImplementationFactory is { } factory)
        {
            return factory(provider);
        }

        return descriptor.ImplementationInstance;
    }
}
