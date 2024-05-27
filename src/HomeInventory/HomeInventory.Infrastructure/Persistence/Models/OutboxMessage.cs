﻿using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Events;
using Visus.Cuid;

namespace HomeInventory.Infrastructure.Persistence.Models;

public record OutboxMessage(Cuid Id, DateTimeOffset OccurredOn, IDomainEvent Content) : IPersistentModel, IHasCreationAudit
{
    public required DateTimeOffset CreatedOn { get; init; }
};
