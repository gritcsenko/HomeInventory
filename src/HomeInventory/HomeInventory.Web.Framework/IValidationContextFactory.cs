using FluentValidation;

namespace HomeInventory.Web.Framework;

public interface IValidationContextFactory<in TObject>
{
    IValidationContext CreateContext(TObject obj);
}
