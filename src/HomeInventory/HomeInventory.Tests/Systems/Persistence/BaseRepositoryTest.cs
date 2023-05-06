using AutoMapper;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseRepositoryTest : BaseDatabaseContextTest
{
    private readonly IDbContextFactory<DatabaseContext> _factory = Substitute.For<IDbContextFactory<DatabaseContext>>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();

    protected BaseRepositoryTest()
    {
        _factory.CreateDbContextAsync(Cancellation.Token).Returns(Context);
    }

    protected private IDbContextFactory<DatabaseContext> Factory => _factory;

    protected IMapper Mapper => _mapper;
}
