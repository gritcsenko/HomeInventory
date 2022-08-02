using FluentValidation;
using FluentValidation.Validators;

namespace HomeInventory.Contracts.Validations;

internal class PasswordValidator<T> : PropertyValidator<T, string>
{
    private readonly IPasswordCharacterSet _requiredSet;

    public PasswordValidator(IPasswordCharacterSet requiredSet)
    {
        _requiredSet = requiredSet;
    }

    public override string Name => "PasswordValidator";

    public override bool IsValid(ValidationContext<T> context, string value)
    {
        if (value == null || _requiredSet.IsEmpty)
        {
            return true;
        }

        return _requiredSet.ContainsAll(value);
    }

    protected override string GetDefaultMessageTemplate(string errorCode) => Localized(errorCode, Name);
}
