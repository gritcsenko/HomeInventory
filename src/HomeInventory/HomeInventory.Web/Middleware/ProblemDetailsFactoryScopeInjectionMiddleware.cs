using HomeInventory.Web.Infrastructure;

namespace HomeInventory.Web.Middleware;

internal class ProblemDetailsFactoryScopeInjectionMiddleware(IProblemDetailsFactory factory, IScopeAccessor scopeAccessor) : BaseScopeInjectionMiddleware<IProblemDetailsFactory>(scopeAccessor)
{
    private readonly IProblemDetailsFactory _factory = factory;
 
    protected override IProblemDetailsFactory GetContext() => _factory;
}
