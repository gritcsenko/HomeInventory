﻿using DotNext;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Ids;
using System.Runtime.CompilerServices;

namespace HomeInventory.Domain.ValueObjects;

public sealed class MeasurementType : BaseEnumeration<MeasurementType, Ulid>
{
    private MeasurementType(string name, ISupplier<Ulid> supplier)
        : base(name, supplier.Invoke())
    {
    }

    public static readonly MeasurementType Count = Create();
    public static readonly MeasurementType Length = Create();
    public static readonly MeasurementType Area = Create();
    public static readonly MeasurementType Volume = Create();
    public static readonly MeasurementType Weight = Create();
    public static readonly MeasurementType Temperature = Create();

    public override string ToString() => Name;

    internal static MeasurementType Create([CallerMemberName] string name = "") => Create(IdSuppliers.Ulid, name);

    internal static MeasurementType Create(ISupplier<Ulid> supplier, [CallerMemberName] string name = "") => new(name, supplier);
}
