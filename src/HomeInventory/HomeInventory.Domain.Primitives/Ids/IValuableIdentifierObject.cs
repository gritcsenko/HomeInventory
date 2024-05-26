﻿using HomeInventory.Application.Mapping;

namespace HomeInventory.Domain.Primitives.Ids;

public interface IValuableIdentifierObject<TSelf, TId> : IIdentifierObject<TSelf>
    where TSelf : IValuableIdentifierObject<TSelf, TId>
    where TId : notnull
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
    static abstract ISupplier<TId> IdSupplier { get; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
    static abstract ObjectConverter<TId, TSelf> Converter { get; }

    TId Value { get; }
}
