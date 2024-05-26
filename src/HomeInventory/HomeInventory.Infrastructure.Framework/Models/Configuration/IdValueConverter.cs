using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Primitives.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal sealed class IdValueConverter<TModel, TProvider, TConverter>(TConverter converter) : ValueConverter<TModel, TProvider>(id => id.Value, value => converter.Convert(value))
    where TModel : IdentifierObject<TModel, TProvider>
    where TProvider : notnull
    where TConverter : ObjectConverter<TModel, TProvider>
{
}
