﻿namespace HomeInventory.Domain.ValueObjects;

public class MeasurementType : Enumeration<MeasurementType, Guid>
{
    private MeasurementType(string name)
        : base(name, Guid.NewGuid())
    {
    }

    public static readonly MeasurementType Count = new(nameof(Count));
    public static readonly MeasurementType Length = new(nameof(Length));
    public static readonly MeasurementType Area = new(nameof(Area));
    public static readonly MeasurementType Volume = new(nameof(Volume));
    public static readonly MeasurementType Weight = new(nameof(Weight));
    public static readonly MeasurementType Temperature = new(nameof(Temperature));
}
