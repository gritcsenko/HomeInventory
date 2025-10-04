using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Modules.Interfaces;

public static class ModuleBuildContextExtensions
{
    public static T GetRequiredService<T>(this IModuleBuildContext context)
        where T : notnull =>
        context.ApplicationBuilder.ApplicationServices.GetRequiredService<T>();
}
