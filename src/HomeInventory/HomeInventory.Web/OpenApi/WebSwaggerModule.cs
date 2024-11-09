using Asp.Versioning;
using HomeInventory.Modules.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace HomeInventory.Web.OpenApi;

public sealed class WebSwaggerModule : BaseModule
{
    public override async Task AddServicesAsync(IModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        context.Services
            .AddEndpointsApiExplorer()
            .ConfigureOptions<ConfigureSwaggerOptions>()
            .AddSingleton<IOpenApiValueConverter, JsonOpenApiValueConverter>()
            .AddSingleton<ISwaggerOperationFilter, DeprecatedSwaggerOperationFilter>()
            .AddSingleton<ISwaggerOperationFilter, ResponsesSwaggerOperationFilter>()
            .AddSingleton<ISwaggerOperationFilter, ParametersSwaggerOperationFilter>()
            .AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>())
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new(1);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new QueryStringApiVersionReader();
            })
            .AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");
    }

    public override async Task BuildAppAsync(IModuleBuildContext context)
    {
        await base.BuildAppAsync(context);

        context.ApplicationBuilder
            .UseSwagger()
            .UseSwaggerUI(options => ConfigureSwaggerUI(context.EndpointRouteBuilder, options));
    }

    private static void ConfigureSwaggerUI(IEndpointRouteBuilder builder, SwaggerUIOptions options)
    {
        var descriptions = builder.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            AddSwaggerEndpoint(options, description.GroupName);
        }
    }

    private static void AddSwaggerEndpoint(SwaggerUIOptions options, string groupName)
    {
        var url = $"/swagger/{groupName}/swagger.json";
        var name = groupName.ToUpperInvariant();
        options.SwaggerEndpoint(url, name);
    }
}
