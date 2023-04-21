using FluentValidation;

namespace HomeInventory.Web.Configuration.Validation;

internal sealed class JwtOptionsValidator : AbstractValidator<JwtOptions>, IOptionsValidator
{
    public JwtOptionsValidator()
    {
        RuleFor(x => x.Secret).NotEmpty();
        RuleFor(x => x.Issuer).NotEmpty();
        RuleFor(x => x.Audience).NotEmpty();
        RuleFor(x => x.Algorithm).NotEmpty();
        RuleFor(x => x.Expiry).GreaterThan(TimeSpan.FromSeconds(1));
    }
}
