using AutoMapper;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseRepositoryTest : BaseDatabaseContextTest
{
    private readonly IDbContextFactory<DatabaseContext> _factory = Substitute.For<IDbContextFactory<DatabaseContext>>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IDateTimeService _dateTimeService = Substitute.For<IDateTimeService>();

    protected BaseRepositoryTest()
    {
        _factory.CreateDbContextAsync(CancellationToken).Returns(Context);
    }

    protected private IDbContextFactory<DatabaseContext> Factory => _factory;

    protected IMapper Mapper => _mapper;
}
