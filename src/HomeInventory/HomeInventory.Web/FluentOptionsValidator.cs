using Carter;
using FluentValidation;
using FluentValidation.Internal;
using Microsoft.Extensions.Options;

namespace HomeInventory.Web;

internal sealed class FluentOptionsValidator<TOptions>(string? name, IValidatorLocator validatorLocator, Action<ValidationStrategy<TOptions>>? validationOptions) : IValidateOptions<TOptions>
    where TOptions : class
{
    private readonly Action<ValidationStrategy<TOptions>> _validationOptions = validationOptions ?? (_ => { });
    private readonly IValidator _validator = validatorLocator.GetValidator<TOptions>();

    public string? Name { get; } = name;

    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        // null name is used to configure all named options
        if (name == Name || name is null)
        {
            return ValidateCore(options);
        }

        // ignored if not validating this instance
        return ValidateOptionsResult.Skip;
    }

    private ValidateOptionsResult ValidateCore(TOptions options)
    {
        var context = ValidationContext<TOptions>.CreateWithOptions(options, _validationOptions);
        var result = _validator.Validate(context);

        return result.IsValid
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(result.ToString());
    }
}
