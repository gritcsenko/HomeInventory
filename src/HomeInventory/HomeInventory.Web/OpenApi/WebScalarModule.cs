using Asp.Versioning;
using HomeInventory.Modules.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace HomeInventory.Web.OpenApi;

public sealed class WebScalarModule : BaseModule
{
    public override async Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken = default)
    {
        await base.AddServicesAsync(context, cancellationToken);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        context.Services
            .AddEndpointsApiExplorer()
            .AddApiVersioning(static options =>
            {
                options.DefaultApiVersion = new(1);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new QueryStringApiVersionReader();
            })
            .AddApiExplorer(static options => options.GroupNameFormat = "'v'VVV")
            .AddOpenApi();
    }

    public override async Task BuildAppAsync(IModuleBuildContext context, CancellationToken cancellationToken = default)
    {
        await base.BuildAppAsync(context, cancellationToken);

        context.EndpointRouteBuilder
            .MapOpenApi()
            .WithDocumentPerVersion();
        context.EndpointRouteBuilder
            .MapScalarApiReference();
    }
}
