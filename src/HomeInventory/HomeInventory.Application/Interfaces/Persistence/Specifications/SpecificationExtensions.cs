using System.Linq.Expressions;
using HomeInventory.Domain.Entities;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public static class SpecificationExtensions
{
    public static FilterSpecification<T> And<T>(this FilterSpecification<T> left, FilterSpecification<T> right)
        where T : notnull, IEntity<T>
        => new BinarySpecification<T>(left, right, Expression.AndAlso);

    public static FilterSpecification<T> Or<T>(this FilterSpecification<T> left, FilterSpecification<T> right)
        where T : notnull, IEntity<T>
        => new BinarySpecification<T>(left, right, Expression.OrElse);

    public static FilterSpecification<T> Not<T>(this FilterSpecification<T> original)
        where T : notnull, IEntity<T>
        => new UnarySpecification<T>(original, Expression.Not);
}
