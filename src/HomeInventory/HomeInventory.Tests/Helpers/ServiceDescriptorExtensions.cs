using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Helpers;

internal static class ServiceDescriptorExtensions
{
    public static object? GetInstance(this ServiceDescriptor descriptor, IServiceProvider provider)
    {
        if (descriptor.ImplementationInstance is { } instance)
        {
            return instance;
        }
        if (descriptor.ImplementationFactory is { } factory)
        {
            instance = factory(provider);
            return instance;
        }
        if (descriptor.ImplementationType is { } type)
        {
            instance = ActivatorUtilities.CreateInstance(provider, type);
            return instance;
        }

        throw new InvalidOperationException($"Cannot create instance for {provider}");
    }
}
