using FluentValidation;
using FluentValidation.Internal;

namespace HomeInventory.Web.Framework;

internal sealed class ValidationContextFactory<TOptions>(Action<ValidationStrategy<TOptions>>? validationOptions = null) : IValidationContextFactory<TOptions>
{
    private readonly Action<ValidationStrategy<TOptions>> _validationOptions = validationOptions ?? (static _ => { });

    public IValidationContext CreateContext(TOptions options) =>
        ValidationContext<TOptions>.CreateWithOptions(options, _validationOptions);
}
