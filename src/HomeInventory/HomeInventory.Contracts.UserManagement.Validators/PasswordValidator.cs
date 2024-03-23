using FluentValidation;
using FluentValidation.Validators;
using HomeInventory.Core;

namespace HomeInventory.Contracts.Validations;

internal sealed class PasswordValidator<T>(IEnumerable<IPasswordCharacterSet> requiredSets) : PropertyValidator<T, string?>
{
    private readonly IEnumerable<IPasswordCharacterSet> _requiredSets = requiredSets.ToArray();

    public override string Name => "PasswordValidator";

    public override bool IsValid(ValidationContext<T> context, string? value) =>
        value == null
        || _requiredSets
            .FirstOrNone(set => !set.ContainsAny(value))
            .Convert(set =>
            {
                context.MessageFormatter.AppendArgument("Category", set.Name);
                return false;
            })
            .Or(true);

    protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must contain {Category} characters";
}
