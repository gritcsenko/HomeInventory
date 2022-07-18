using HomeInventory.Domain.Entities;
using System.Linq.Expressions;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public static class SpecificationExtensions
{
    public static FilterSpecification<T> And<T>(this FilterSpecification<T> left, FilterSpecification<T> right)
        where T : notnull, IEntity<T>
        => new BinarySpecification<T>(left, right, Expression.AndAlso);

    public static FilterSpecification<T> Or<T>(this FilterSpecification<T> left, FilterSpecification<T> right)
        where T : notnull, IEntity<T>
        => new BinarySpecification<T>(left, right, Expression.OrElse);
}
