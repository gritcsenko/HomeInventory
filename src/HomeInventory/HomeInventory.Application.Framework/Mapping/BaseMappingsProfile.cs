using AutoMapper;
using System.Linq.Expressions;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Framework.Mapping;

public abstract class BaseMappingsProfile : Profile
{
    protected IObjectMap<TSource> CreateMap<TSource>() => new ObjectMap<TSource>(this);

    protected interface IObjectMap<TSource>
    {
        IMappingExpression<TSource, TDestination> To<TDestination>();

        IMappingExpression<TSource, TDestination> Using<TDestination>(Func<TSource, ResolutionContext, TDestination> convertTo);

        void Using<TDestination>(Expression<Func<TSource, TDestination>> convertTo, Expression<Func<TDestination, TSource>> convertFrom);

        void Using<TDestination>(Expression<Func<TSource, TDestination>> convertTo, ObjectConverter<TDestination, TSource> converterFrom);

        void Using<TDestination, TFromValueConverter>(Expression<Func<TSource, TDestination>> convertTo)
            where TFromValueConverter : ObjectConverter<TDestination, TSource>;
    }

    private sealed class ObjectMap<TSource>(Profile profile) : IObjectMap<TSource>
    {
        private readonly Profile _profile = profile;

        public IMappingExpression<TSource, TDestination> To<TDestination>() =>
            _profile.CreateMap<TSource, TDestination>();

        public IMappingExpression<TSource, TDestination> Using<TDestination>(Func<TSource, ResolutionContext, TDestination> convertTo) =>
            To<TDestination>()
                .ConstructUsing(convertTo);

        public void Using<TDestination>(Expression<Func<TSource, TDestination>> convertTo, Expression<Func<TDestination, TSource>> convertFrom)
        {
            _profile.CreateMap<TSource, TDestination>()
                .ConvertUsing(convertTo);
            _profile.CreateMap<TDestination, TSource>()
                .ConvertUsing(convertFrom);
        }

        public void Using<TDestination>(Expression<Func<TSource, TDestination>> convertTo, ObjectConverter<TDestination, TSource> converterFrom)
        {
            _profile.CreateMap<TSource, TDestination>()
                .ConvertUsing(convertTo);
            _profile.CreateMap<TDestination, TSource>()
                .ConvertUsing(new TypeConverterAdapter<TDestination, TSource, ObjectConverter<TDestination, TSource>>(converterFrom));
        }

        public void Using<TDestination, TFromValueConverter>(Expression<Func<TSource, TDestination>> convertTo)
            where TFromValueConverter : ObjectConverter<TDestination, TSource>
        {
            _profile.CreateMap<TSource, TDestination>()
                .ConvertUsing(convertTo);
            _profile.CreateMap<TDestination, TSource>()
                .ConvertUsing<TypeConverterAdapter<TDestination, TSource, TFromValueConverter>>();
        }
    }
}
