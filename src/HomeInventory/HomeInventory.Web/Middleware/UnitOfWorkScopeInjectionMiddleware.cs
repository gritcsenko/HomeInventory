using HomeInventory.Core;
using HomeInventory.Domain.Primitives;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web.Middleware;

internal class UnitOfWorkScopeInjectionMiddleware(IUnitOfWork unitOfWork, IScopeAccessor scopeAccessor) : IMiddleware
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        using (_scopeAccessor.Set(_unitOfWork))
        {
            await next(context);
        }
    }
}
