using FluentValidation;
using FluentValidation.Validators;
using LanguageExt;

namespace HomeInventory.Contracts.UserManagement.Validators;

internal sealed class PasswordValidator<T>(IEnumerable<IPasswordCharacterSet> requiredSets) : PropertyValidator<T, string?>
{
    private readonly IEnumerable<IPasswordCharacterSet> _requiredSets = requiredSets.ToArray();

    public override string Name => "PasswordValidator";

    public override bool IsValid(ValidationContext<T> context, string? value) =>
        value == null
        || _requiredSets
            .Where(set => !set.ContainsAny(value))
            .ToOption()
            .Map(set =>
            {
                context.MessageFormatter.AppendArgument("Category", set.Name);
                return false;
            })
            .IfNone(true);

    protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must contain {Category} characters";
}
