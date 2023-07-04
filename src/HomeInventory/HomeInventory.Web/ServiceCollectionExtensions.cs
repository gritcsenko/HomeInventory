using System.Reflection;
using Asp.Versioning;
using Carter;
using DotNext;
using FluentValidation.Internal;
using HealthChecks.ApplicationStatus.DependencyInjection;
using HealthChecks.UI.Client;
using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Web.Authentication;
using HomeInventory.Web.Authorization.Dynamic;
using HomeInventory.Web.Configuration;
using HomeInventory.Web.Configuration.Interfaces;
using HomeInventory.Web.Framework;
using HomeInventory.Web.Middleware;
using HomeInventory.Web.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace HomeInventory.Web;

public static class ServiceCollectionExtensions
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

        services.AddMappingAssemblySource(AssemblyReference.Assembly);
        services.AddAutoMapper((sp, configExpression) =>
        {
            configExpression.AddMaps(sp.GetServices<IMappingAssemblySource>().Select(s => s.GetAssembly()));
            configExpression.ConstructServicesUsing(sp.GetService);
        }, Type.EmptyTypes);

        services.AddAuthorization();

        services.AddOpenApiDocs();

        var coreAssemblies = new[]{
            Contracts.Validations.AssemblyReference.Assembly,
            Contracts.UserManagement.Validators.AssemblyReference.Assembly,
            AssemblyReference.Assembly
        };
        services.AddCarter(new DependencyContextAssemblyCatalog(coreAssemblies.Concat(moduleAssemblies).ToArray()));

        return services;
    }

    private static void AddAuthorization(this IServiceCollection services)
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
        app.Map("/error", (HttpContext context) => Results.Problem(detail: context.GetFeature<IExceptionHandlerFeature>()?.Error?.Message));

        app.UseMiddleware<CorrelationIdMiddleware>();

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

    private static OptionsBuilder<TOptions> AddOptionsWithValidator<TOptions>(this IServiceCollection services, string? configSectionPath = null)
        where TOptions : class => services.AddOptions<TOptions>()
            .BindConfiguration(configSectionPath ?? typeof(TOptions).Name)
            .ValidateWithValidator()
            .ValidateOnStart();

    private static OptionsBuilder<TOptions> ValidateWithValidator<TOptions>(this OptionsBuilder<TOptions> builder, Action<ValidationStrategy<TOptions>>? validationOptions = null)
        where TOptions : class
    {
        var services = builder.Services;
        services.AddSingleton<IValidateOptions<TOptions>>(sp => new FluentOptionsValidator<TOptions>(builder.Name, sp.GetRequiredService<IValidatorLocator>(), validationOptions));
        return builder;
    }
}
