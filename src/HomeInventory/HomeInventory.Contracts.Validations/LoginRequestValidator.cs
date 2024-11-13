using FluentValidation;

namespace HomeInventory.Contracts.Validations;

internal sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(static x => x.Email).NotEmpty();
        RuleFor(static x => x.Email).EmailAddress();

        RuleFor(static x => x.Password).NotEmpty();
    }
}
