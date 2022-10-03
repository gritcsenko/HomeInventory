using FluentValidation;

namespace HomeInventory.Contracts.Validations;
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();

        RuleFor(x => x.LastName).NotEmpty();

        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();

        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.Password).MinimumLength(8);
        RuleFor(x => x.Password).Password();
    }
}
