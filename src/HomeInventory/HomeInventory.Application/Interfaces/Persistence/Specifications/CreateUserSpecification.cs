using DotNext;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public record CreateUserSpecification(Email Email, string Password, ISupplier<Guid> UserIdSupplier) : ICreateEntitySpecification<User>;
