using FluentValidation;

namespace HomeInventory.Contracts.Validations;

internal class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();

        RuleFor(x => x.Password).NotEmpty();
    }
}
