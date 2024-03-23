using FluentValidation;
using FluentValidation.Validators;

namespace HomeInventory.Contracts.Validations;

internal class PasswordValidator<T>(IEnumerable<IPasswordCharacterSet> requiredSets) : PropertyValidator<T, string?>
{
    private readonly IEnumerable<IPasswordCharacterSet> _requiredSets = requiredSets.ToArray();

    public override string Name => "PasswordValidator";

    public override bool IsValid(ValidationContext<T> context, string? value)
    {
        if (value == null)
        {
            return true;
        }

        foreach (var requiredSet in _requiredSets)
        {
            if (!requiredSet.ContainsAny(value))
            {
                context.MessageFormatter.AppendArgument("Category", requiredSet.Name);
                return false;
            }
        }

        return true;
    }

    protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must contain {Category} characters";
}
