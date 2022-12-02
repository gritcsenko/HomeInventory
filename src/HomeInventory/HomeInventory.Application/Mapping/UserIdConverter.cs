using AutoMapper;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Mapping;

public class UserIdConverter : IValueConverter<Guid, UserId>, ITypeConverter<Guid, UserId>
{
    private readonly IIdFactory<UserId, Guid> _factory;

    public UserIdConverter(IIdFactory<UserId, Guid> factory) => _factory = factory;

    public UserId Convert(Guid sourceMember, ResolutionContext context) => (UserId)_factory.CreateFrom(sourceMember).Value;

    public UserId Convert(Guid source, UserId destination, ResolutionContext context) => (UserId)_factory.CreateFrom(source).Value;
}
