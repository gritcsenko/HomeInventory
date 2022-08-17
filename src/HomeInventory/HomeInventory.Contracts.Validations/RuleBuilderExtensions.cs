using FluentValidation;

namespace HomeInventory.Contracts.Validations;

internal static class RuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string?> Password<T>(this IRuleBuilder<T, string?> ruleBuilder, Action<PasswordValidatorOptions>? setOptions = null)
    {
        var options = new PasswordValidatorOptions();
        setOptions?.Invoke(options);
        var validator = new PasswordValidator<T>(options.GetCharacterSets());
        return ruleBuilder.SetValidator(validator);
    }
}
