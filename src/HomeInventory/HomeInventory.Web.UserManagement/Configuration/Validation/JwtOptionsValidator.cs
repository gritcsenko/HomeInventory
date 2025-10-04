using FluentValidation;

namespace HomeInventory.Web.UserManagement.Configuration.Validation;

internal sealed class JwtOptionsValidator : AbstractValidator<JwtOptions>
{
    public JwtOptionsValidator()
    {
        RuleFor(static x => x.Secret).NotEmpty();
        RuleFor(static x => x.Issuer).NotEmpty();
        RuleFor(static x => x.Audience).NotEmpty();
        RuleFor(static x => x.Algorithm).NotEmpty();
        RuleFor(static x => x.Expiry).GreaterThan(TimeSpan.FromSeconds(1));
    }
}
