using FluentValidation;
using FluentValidation.Internal;

namespace HomeInventory.Web.Framework;

internal sealed class ValidationContextFactory<TObject>(Action<ValidationStrategy<TObject>>? validationOptions = null) : IValidationContextFactory<TObject>
{
    private readonly Action<ValidationStrategy<TObject>> _validationOptions = validationOptions ?? (static _ => { });

    public IValidationContext CreateContext(TObject obj) =>
        ValidationContext<TObject>.CreateWithOptions(obj, _validationOptions);
}
