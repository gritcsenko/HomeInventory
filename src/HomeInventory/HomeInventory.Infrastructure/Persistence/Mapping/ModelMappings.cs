using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using Mapster;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

internal class ModelMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserId, Guid>().MapWith(id => id.Id);
        config.NewConfig<Guid, UserId>()
            .ConstructUsing(id => CreateUserId(MapContext.Current.GetService<IIdFactory<UserId, Guid>>(), id));
        config.NewConfig<User, UserModel>();
        config.NewConfig<UserModel, User>()
            .ConstructUsing(m => new User(CreateUserId(MapContext.Current.GetService<IIdFactory<UserId, Guid>>(), m.Id)));
    }

    private static UserId CreateUserId(IIdFactory<UserId, Guid> factory, Guid id) => factory.CreateFrom(id).Value;
}
