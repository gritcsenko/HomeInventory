using Asp.Versioning;
using HomeInventory.Modules.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace HomeInventory.Web.OpenApi;

public sealed class WebSwaggerModule : BaseModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.ConfigureOptions<ConfigureSwaggerOptions>();
        services.AddSingleton<IOpenApiValueConverter, JsonOpenApiValueConverter>();
        services.AddSingleton<ISwaggerOperationFilter, DeprecatedSwaggerOperationFilter>();
        services.AddSingleton<ISwaggerOperationFilter, ResponsesSwaggerOperationFilter>();
        services.AddSingleton<ISwaggerOperationFilter, ParametersSwaggerOperationFilter>();
        services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new QueryStringApiVersionReader();
        }).AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");
    }

    public override void BuildApp(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder)
    {
        applicationBuilder.UseSwagger();
        applicationBuilder.UseSwaggerUI(options => ConfigureSwaggerUI(endpointRouteBuilder, options));
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
