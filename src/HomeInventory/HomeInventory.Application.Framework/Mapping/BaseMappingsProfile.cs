using AutoMapper;
using System.Linq.Expressions;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Application.Framework.Mapping;

public abstract class BaseMappingsProfile : Profile
{
    protected BaseMappingsProfile()
    {
    }

    protected MapBuilder<TSource> CreateMap<TSource>() => new(this);
}

public abstract class BaseMapBuilder<TSource>(Profile profile)
{
    protected Profile Profile { get; } = profile;

    public IMappingExpression<TSource, TDestination> To<TDestination>() =>
        Profile.CreateMap<TSource, TDestination>();

    public IMappingExpression<TDestination, TSource> From<TDestination>() =>
        Profile.CreateMap<TDestination, TSource>();
}

public sealed class MapBuilder<TSource>(Profile profile) : BaseMapBuilder<TSource>(profile)
{
    public MapBuilder<TSource, TDependency> With<TDependency>() => new(Profile);

    public IMappingExpression<TSource, TDestination> Using<TDestination>(Func<TSource, ResolutionContext, TDestination> convertTo) =>
        To<TDestination>().ConstructUsing(convertTo);

    public void Using<TDestination>(Expression<Func<TSource, TDestination>> convertTo, Expression<Func<TDestination, TSource>> convertFrom)
    {
        To<TDestination>().ConvertUsing(convertTo);
        From<TDestination>().ConvertUsing(convertFrom);
    }

    public void Using<TDestination>(Expression<Func<TSource, TDestination>> convertTo, ObjectConverter<TDestination, TSource> converterFrom)
    {
        To<TDestination>().ConvertUsing(convertTo);
        From<TDestination>().ConvertUsing(new TypeConverterAdapter<TDestination, TSource, ObjectConverter<TDestination, TSource>>(converterFrom));
    }

    public void Using<TDestination, TFromValueConverter>(Expression<Func<TSource, TDestination>> convertTo)
        where TFromValueConverter : ObjectConverter<TDestination, TSource>
    {
        To<TDestination>().ConvertUsing(convertTo);
        From<TDestination>().ConvertUsing<TypeConverterAdapter<TDestination, TSource, TFromValueConverter>>();
    }
}

public sealed class MapBuilder<TSource, TDependency>(Profile profile) : BaseMapBuilder<TSource>(profile)
{
    public MapBuilder<TSource, TDependency> Using<TDestination>(Func<TSource, TDependency, IRuntimeMapper, TDestination> convertTo)
    {
        To<TDestination>().ConstructUsing((src, ctx) =>
        {
            var state = ctx.State ?? throw new InvalidOperationException("No dependency specificed");
            if (state is not TDependency dependency)
            {
                throw new InvalidOperationException($"Cannot convert state {state} of type {state.GetType()} to {typeof(TDependency)}");
            }

            return convertTo(src, dependency, ctx.Mapper);
        });

        return this;
    }
    public MapBuilder<TSource, TDependency> Using<TDestination, TResult>(Func<TSource, TDependency, IRuntimeMapper, TDestination> convertTo)
        where TDestination : IRequestMessage<IQueryResult<TResult>>
        where TResult : notnull
    {
        To<TDestination>().ConstructUsing((src, ctx) =>
        {
            var state = ctx.State ?? throw new InvalidOperationException("No dependency specificed");
            if (state is not TDependency dependency)
            {
                throw new InvalidOperationException($"Cannot convert state {state} of type {state.GetType()} to {typeof(TDependency)}");
            }

            return convertTo(src, dependency, ctx.Mapper);
        });

        return this;
    }
}
