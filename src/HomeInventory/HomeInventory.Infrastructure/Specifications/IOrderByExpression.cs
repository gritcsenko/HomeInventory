using System.Linq.Expressions;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal interface IOrderByExpression<TModel>
    where TModel : class, IPersistentModel
{
    Expression<Func<TModel, object>> Expression { get; }

    bool IsDescending { get; }
}
