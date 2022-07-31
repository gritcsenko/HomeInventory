using HomeInventory.Tests.Helpers;
using Mapster;
using MapsterMapper;

namespace HomeInventory.Tests.Systems.Mapping;

public abstract class BaseMappingsTests : BaseTest
{
    protected BaseMappingsTests()
    {
    }

    protected static IMapper CreateSut<TMapper>()
        where TMapper : IRegister, new()
    {
        var config = new TypeAdapterConfig();
        config.Apply(new TMapper());
        return new Mapper(config);
    }
}
