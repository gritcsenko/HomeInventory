using Asp.Versioning.Builder;
using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Extensions;

internal static class EndpointRouteBuilderExtensions
{
    public static ApiVersionSet GetVersionSet(this IEndpointRouteBuilder app) =>
        app.NewApiVersionSet()
            .HasApiVersion(1)
            .ReportApiVersions()
            .Build();
}
