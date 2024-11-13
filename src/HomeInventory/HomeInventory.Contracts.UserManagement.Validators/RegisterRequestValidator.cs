using FluentValidation;

namespace HomeInventory.Contracts.Validations;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(static x => x.Email).NotEmpty();
        RuleFor(static x => x.Email).EmailAddress();

        RuleFor(static x => x.Password).NotEmpty();
        RuleFor(static x => x.Password).MinimumLength(8);
        RuleFor(static x => x.Password).Password();
    }
}
