using HomeInventory.Web.Framework;
using HomeInventory.Web.Framework.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Tests.Presentation.Web;

public sealed class ValidationEndpointFilterTestsGivenContext(BaseTest test) : GivenContext<ValidationEndpointFilterTestsGivenContext, IEndpointFilter, IValidationContextFactory<Guid>, IProblemDetailsFactory>(test)
{
    protected override IEndpointFilter CreateSut(IValidationContextFactory<Guid> arg1, IProblemDetailsFactory arg2) => new ValidationEndpointFilter<Guid>(arg1, arg2);
}
