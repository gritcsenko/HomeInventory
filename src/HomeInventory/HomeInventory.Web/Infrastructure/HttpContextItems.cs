using DotNext;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web.Infrastructure;

internal static class HttpContextItems
{
    public static HttpContextItem<IEnumerable<IError>> Errors { get; } = new(nameof(Errors));

    public static void SetItem<T>(this HttpContext context, HttpContextItem<T> item, Optional<T> optional)
    {
        if (optional.TryGet(out var value))
        {
            context.Items[item.Name] = value;
        }
    }

    public static Optional<T> GetItem<T>(this HttpContext context, HttpContextItem<T> item) =>
        context.Items.GetValueOptional(item.Name)
            .Convert(x => new Optional<object>(x))
            .Convert(x => (T)x);

    public record HttpContextItem<T>(string Name);
}
