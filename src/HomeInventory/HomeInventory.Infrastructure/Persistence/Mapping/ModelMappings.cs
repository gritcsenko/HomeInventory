using ErrorOr;
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
            .ConstructUsing(id => MapContext.Current.GetService<IUserIdFactory>().Create(id)
                .Match(x => x, ReportError<UserId>)
            );
        config.NewConfig<User, UserModel>();
        config.NewConfig<UserModel, User>();
    }

    private static T ReportError<T>(IReadOnlyCollection<Error> errors)
    {
        throw new InvalidOperationException(errors.First().ToString());
    }
}
