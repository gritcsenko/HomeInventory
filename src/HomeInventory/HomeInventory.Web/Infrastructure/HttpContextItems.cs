using HomeInventory.Domain.Errors;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web.Infrastructure;

internal static class HttpContextItems
{
    public static HttpContextItem<IEnumerable<IError>> Errors { get; } = new HttpContextItem<IEnumerable<IError>> { Name = nameof(Errors) };

    public static void SetItem<T>(this HttpContext context, HttpContextItem<T> item, T? value)
        where T : class
    {
        context.Items[item.Name] = value;
    }

    public static T? GetItem<T>(this HttpContext context, HttpContextItem<T> item)
        where T : class
    {
        return context.Items[item.Name] as T;
    }

    public readonly struct HttpContextItem<T>
        where T : class
    {
        public string Name { get; init; }
    }
}
