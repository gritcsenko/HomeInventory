using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Microsoft.Extensions.Options;

namespace HomeInventory.Web.Framework;

internal static class FluentOptionsValidator
{
    public static IValidateOptions<TOptions> Create<TOptions>(string name, IValidator validator, Action<ValidationStrategy<TOptions>>? validationOptions = null)
        where TOptions : class
    {
        var factory = new ValidationContextFactory<TOptions>(validationOptions);
        return new FluentOptionsValidator<TOptions>(name, validator, factory);
    }
}

internal sealed class FluentOptionsValidator<TOptions>(string name, IValidator validator, IValidationContextFactory<TOptions> validationContextFactory) : IValidateOptions<TOptions>
    where TOptions : class
{
    private readonly string _name = name;
    private readonly IValidator _validator = validator;
    private readonly IValidationContextFactory<TOptions> _validationContextFactory = validationContextFactory;

    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        if (ShouldSkip(name))
        {
            return ValidateOptionsResult.Skip;
        }

        var context = _validationContextFactory.CreateContext(options);
        var result = _validator.Validate(context);

        return ToValidateOptionsResult(result);
    }

    private bool ShouldSkip(string? name)
    {
        var validateAll = name is null;
        return !validateAll && name != _name;
    }

    private static ValidateOptionsResult ToValidateOptionsResult(ValidationResult result) =>
        result.IsValid
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(result.ToString());
}
