using AutoMapper;

namespace HomeInventory.Web.Middleware;

internal class MapperScopeInjectionMiddleware(IMapper mapper, IScopeAccessor scopeAccessor) : BaseScopeInjectionMiddleware<IMapper>(scopeAccessor)
{
    private readonly IMapper _mapper = mapper;

    protected override IMapper GetContext() => _mapper;
}
