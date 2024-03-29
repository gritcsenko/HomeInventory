﻿using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class Email(string value) : ValueObject<Email>(value)
{
    public string Value => GetComponent<string>(0);

    public override string ToString() => Value;
}