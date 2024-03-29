﻿using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Domain.ValueObjects;

public interface IAmountFactory
{
    OneOf<Amount, IError> Create(decimal value, AmountUnit unit);
}
