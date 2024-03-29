﻿using System.Linq.Expressions;
using AutoMapper;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Mapping;

public abstract class MappingProfile : Profile
{
    protected MappingProfile()
    {
    }

    protected void CreateMapForId<TId>()
        where TId : class, IUlidBuildable<TId>, IUlidIdentifierObject<TId>
    {
        var converter = new UlidIdConverter<TId>();
        CreateMap<TId, Ulid>()
            .ConvertUsing(x => x.Value);
        CreateMap<Ulid, TId>()
            .ConvertUsing(id => converter.Convert(id));
    }

    protected void CreateMapForString<TObject>(Expression<Func<string, TObject>> convertFromValue, Expression<Func<TObject, string>> convertToValue)
        where TObject : class, IValueObject<TObject>
    {
        CreateMap<TObject, string>()
            .ConvertUsing(convertToValue);
        CreateMap<string, TObject>()
            .ConvertUsing(convertFromValue);
    }

    protected void CreateMapForValue<TObject, TValue, TConverter>(Expression<Func<TObject, TValue>> convertToValue)
        where TObject : class, IValueObject<TObject>
        where TConverter : ITypeConverter<TValue, TObject>
    {
        CreateMap<TObject, TValue>()
            .ConvertUsing(convertToValue);
        CreateMap<TValue, TObject>()
            .ConvertUsing<TConverter>();
    }
}
