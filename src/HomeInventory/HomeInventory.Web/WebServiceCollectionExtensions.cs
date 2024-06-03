using System.Reflection;
using Asp.Versioning;
using Carter;
using HealthChecks.ApplicationStatus.DependencyInjection;
using HealthChecks.UI.Client;
using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Web.Authentication;
using HomeInventory.Web.Configuration;
using HomeInventory.Web.Configuration.Interfaces;
using HomeInventory.Web.Middleware;
using HomeInventory.Web.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebServiceCollectionExtensions
{
    public static IServiceCollection AddWeb(this IServiceCollection services, params Assembly[] moduleAssemblies)
    {
        // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health
        // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0
        services.AddHealthChecks()
            .AddApplicationStatus();
        services.AddHealthChecksUI()
            .AddInMemoryStorage();

        services.AddWebFramework();

        services.AddScoped<ICorrelationIdContainer, CorrelationIdContainer>();
        services.AddScoped<CorrelationIdMiddleware>();
        services.AddScoped<MapperScopeInjectionMiddleware>();
        services.AddScoped<UnitOfWorkScopeInjectionMiddleware>();
        services.AddScoped<ProblemTraceIdentifierMiddleware>();
        services.AddScoped<MessageHubScopeInjectionMiddleware>();
        services.AddScoped<ProblemDetailsFactoryScopeInjectionMiddleware>();

        services.AddMappingAssemblySource(moduleAssemblies);
        services.AddAutoMapper((sp, configExpression) =>
        {
            configExpression.AddMaps(sp.GetServices<IMappingAssemblySource>().SelectMany(s => s.GetAssemblies()));
            configExpression.ConstructServicesUsing(sp.GetService);
        }, Type.EmptyTypes);

        services.AddWebAuthorization();

        services.AddOpenApiDocs();

        services.AddCarter(new DependencyContextAssemblyCatalog(moduleAssemblies));

        return services;
    }

    private static void AddWebAuthorization(this IServiceCollection services)
    {
        services.AddDynamicAuthorization();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddSingleton<IJwtIdentityGenerator, GuidJwtIdentityGenerator>();
        services.AddOptionsWithValidator<JwtOptions>();
        services.AddScoped<IAuthenticationTokenGenerator, JwtTokenGenerator>();
    }

    private static void AddOpenApiDocs(this IServiceCollection services)
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

    public static TApp UseWeb<TApp>(this TApp app)
        where TApp : IApplicationBuilder, IEndpointRouteBuilder
    {
        app.UseSwagger();
        app.UseSwaggerUI(app.ConfigureSwaggerUI);

        app.MapHealthChecks();
        app.UseHealthChecksUI();

        app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandlingPath = "/error", });
        app.Map("/error", (HttpContext context) => Results.Problem(detail: context.GetFeature<IExceptionHandlerPathFeature>()?.Error?.Message));

        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ProblemTraceIdentifierMiddleware>();
        app.UseMiddleware<MapperScopeInjectionMiddleware>();
        app.UseMiddleware<UnitOfWorkScopeInjectionMiddleware>();
        app.UseMiddleware<MessageHubScopeInjectionMiddleware>();
        app.UseMiddleware<ProblemDetailsFactoryScopeInjectionMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseDynamicAuthorization();

        app.MapCarter();

        return app;
    }

    private static TFeature? GetFeature<TFeature>(this HttpContext context) =>
      context.Features.Get<TFeature>();

    private static void ConfigureSwaggerUI(this IEndpointRouteBuilder builder, SwaggerUIOptions options)
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

    private static void MapHealthChecks(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = x => x.Tags.Contains(HealthCheckTags.Ready),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        });
    }
}
