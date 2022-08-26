﻿using ErrorOr;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using Mapster;

namespace HomeInventory.Infrastructure.Persistence.Mapping;
internal class ModelMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserId, Guid>().MapWith(id => (Guid)id);
        config.NewConfig<Guid, UserId>()
            .ConstructUsing(id => CreateUserId(MapContext.Current.GetService<IUserIdFactory>(), id));
        config.NewConfig<User, UserModel>();
        config.NewConfig<UserModel, User>()
            .ConstructUsing(m => new User(CreateUserId(MapContext.Current.GetService<IUserIdFactory>(), m.Id)));
    }

    private static UserId CreateUserId(IUserIdFactory factory, Guid id) =>
        factory.Create(id).Match(x => x, ReportError<UserId>);

    private static T ReportError<T>(IReadOnlyCollection<Error> errors) =>
        throw new InvalidOperationException(errors.First().ToString());
}
