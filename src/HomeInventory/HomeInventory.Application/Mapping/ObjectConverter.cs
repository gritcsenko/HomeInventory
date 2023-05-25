﻿using DotNext;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Application.Mapping;

public class ObjectConverter<TObject, TValue> : ObjectConverter<ValueObject<TObject>.Builder<TValue>, TObject, TValue>
    where TValue : notnull
    where TObject : notnull, ValueObject<TObject>, IBuildable<TObject, ValueObject<TObject>.Builder<TValue>>
{
    public ObjectConverter(Func<TValue, bool> isValid)
        : base(isValid)
    {
    }
}

public class ObjectConverter<TBuilder, TObject, TValue> : GenericValueObjectConverter<TObject, TValue>
    where TValue : notnull
    where TBuilder : IValueObjectBuilder<TBuilder, TObject, TValue>
    where TObject : notnull, ValueObject<TObject>, IBuildable<TObject, TBuilder>
{
    private readonly Func<TValue, bool> _isValid;

    public ObjectConverter(Func<TValue, bool> isValid)
    {
        _isValid = isValid;
    }

    protected override OneOf<TObject, IError> InternalConvert(TValue source)
    {
        if (!_isValid(source))
        {
            return new ObjectValidationError<TValue>(source);
        }

#pragma warning disable CA2252 // This API requires opting into preview features
        var builder = TObject.CreateBuilder();
#pragma warning restore CA2252 // This API requires opting into preview features
        builder.WithValue(new ValueSupplier<TValue>(source));
        return builder.Invoke();
    }
}