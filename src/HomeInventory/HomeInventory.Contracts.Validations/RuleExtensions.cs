using FluentValidation;

namespace HomeInventory.Contracts.Validations;

internal static class RuleExtensions
{
    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder, Action<PasswordValidatorOptions>? setOptions = null)
    {
        var options = new PasswordValidatorOptions();
        setOptions?.Invoke(options);
        var validator = new PasswordValidator<T>(options.GetCharacterSet());
        return ruleBuilder.SetValidator(validator);
    }
}
