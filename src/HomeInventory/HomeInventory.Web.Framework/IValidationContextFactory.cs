using FluentValidation;

namespace HomeInventory.Web.Framework;

internal interface IValidationContextFactory<in TOptions>
{
    IValidationContext CreateContext(TOptions options);
}
