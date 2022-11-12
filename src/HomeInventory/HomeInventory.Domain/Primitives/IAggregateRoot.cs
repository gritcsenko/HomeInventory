﻿using HomeInventory.Domain.Events;

namespace HomeInventory.Domain.Primitives;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> GetEvents();

    void ClearEvents();
}
