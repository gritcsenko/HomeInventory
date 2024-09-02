using FluentValidation.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HomeInventory.Web.Framework;

public static class WebFrameworkServiceCollectionExtensions
{
    public static OptionsBuilder<TOptions> AddOptionsWithValidator<TOptions>(this IServiceCollection services, Action<ValidationStrategy<TOptions>>? validationOptions = null)
        where TOptions : class, IOptions =>
        services
            .AddOptionsWithValidator(TOptions.Section, validationOptions);

    private static OptionsBuilder<TOptions> AddOptionsWithValidator<TOptions>(this IServiceCollection services, SectionPath configSectionPath, Action<ValidationStrategy<TOptions>>? validationOptions)
        where TOptions : class =>
        services
            .AddOptionsSection<TOptions>(configSectionPath)
            .ValidateWithValidator(validationOptions);

    private static OptionsBuilder<TOptions> AddOptionsSection<TOptions>(this IServiceCollection services, SectionPath configSectionPath)
        where TOptions : class =>
        services
            .AddOptions<TOptions>()
            .BindConfiguration(configSectionPath);

    private static OptionsBuilder<TOptions> ValidateWithValidator<TOptions>(this OptionsBuilder<TOptions> builder, Action<ValidationStrategy<TOptions>>? validationOptions)
        where TOptions : class =>
        builder
            .ValidateWithFluentOptionsValidator(validationOptions)
            .ValidateOnStart();

    private static OptionsBuilder<TOptions> ValidateWithFluentOptionsValidator<TOptions>(this OptionsBuilder<TOptions> builder, Action<ValidationStrategy<TOptions>>? validationOptions)
        where TOptions : class
    {
        var services = builder.Services;
        var name = builder.Name;
        services.AddSingleton(sp => FluentOptionsValidator.Create(name, sp.GetValidator<TOptions>(), validationOptions));
        return builder;
    }
}
