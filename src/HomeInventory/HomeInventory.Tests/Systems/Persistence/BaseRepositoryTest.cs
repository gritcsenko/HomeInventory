using AutoMapper;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseRepositoryTest : BaseDatabaseContextTest
{
    private readonly IMapper _mapper = Substitute.For<IMapper>();

    protected BaseRepositoryTest()
    {
    }

    protected IMapper Mapper => _mapper;
}
