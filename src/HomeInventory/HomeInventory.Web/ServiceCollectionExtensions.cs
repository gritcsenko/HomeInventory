using Asp.Versioning;
using Carter;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Internal;
using HealthChecks.UI.Client;
using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Web.Authentication;
using HomeInventory.Web.Authorization.Dynamic;
using HomeInventory.Web.Configuration;
using HomeInventory.Web.Configuration.Interfaces;
using HomeInventory.Web.Configuration.Validation;
using HomeInventory.Web.Infrastructure;
using HomeInventory.Web.Middleware;
using HomeInventory.Web.Modules;
using HomeInventory.Web.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HomeInventory.Web;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health
        // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0
        services.AddHealthChecks();
        services.AddHealthChecksUI()
            .AddInMemoryStorage();

        services.AddSingleton<ErrorMapping>();
        services.AddSingleton<HomeInventoryProblemDetailsFactory>();
        services.AddSingleton<ProblemDetailsFactory>(sp => sp.GetRequiredService<HomeInventoryProblemDetailsFactory>());
        services.AddScoped<ICorrelationIdContainer, CorrelationIdContainer>();
        services.AddScoped<CorrelationIdMiddleware>();

        services.AddAutoMapper((sp, configExpression) =>
        {
            configExpression.AddMaps(sp.GetServices<IMappingAssemblySource>().Select(s => s.GetAssembly()));
            configExpression.ConstructServicesUsing(sp.GetService);
        }, Type.EmptyTypes);

        services.AddMappingAssemblySource(AssemblyReference.Assembly);

        services.AddDynamicAuthorization();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.ConfigureOptions<ConfigureSwaggerOptions>();
        services.AddSingleton<IOpenApiValueConverter, JsonOpenApiValueConverter>();
        services.AddSingleton<ISwaggerOperationFilter, DeprecatedSwaggerOperationFilter>();
        services.AddSingleton<ISwaggerOperationFilter, ResponsesSwaggerOperationFilter>();
        services.AddSingleton<ISwaggerOperationFilter, ParametersSwaggerOperationFilter>();
        services.AddSwaggerGen(options =>
            options.OperationFilter<SwaggerDefaultValues>());
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new QueryStringApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
        });

        services.AddSingleton<IJwtIdentityGenerator, GuidJwtIdentityGenerator>();
        services.AddScoped<IAuthenticationTokenGenerator, JwtTokenGenerator>();

        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddFluentValidationAutoValidation(c =>
        {
            c.DisableDataAnnotationsValidation = true;
        });
        services.AddValidatorsFromAssembly(Contracts.Validations.AssemblyReference.Assembly, includeInternalTypes: true);
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly, ServiceLifetime.Singleton, filter: r => r.Is<IOptionsValidator>(), includeInternalTypes: true);
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly, ServiceLifetime.Scoped, filter: r => !r.Is<IOptionsValidator>(), includeInternalTypes: true);

        services.AddOptionsWithValidator<JwtOptions>();

        services.AddCarter(configurator: config => config
            .WithModule<AreaModule>()
            .WithModule<UserManagementModule>()
            .WithModule<AuthenticationModule>()
            .WithModule<PermissionModule>());

        return services;
    }

    public static TApp UseWeb<TApp>(this TApp app)
        where TApp : IApplicationBuilder, IEndpointRouteBuilder
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var descriptions = app.DescribeApiVersions();
            foreach (var description in descriptions)
            {
                AddSwaggerEndpoint(options, description);
            }

            static void AddSwaggerEndpoint(Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIOptions options, Asp.Versioning.ApiExplorer.ApiVersionDescription description)
            {
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);
            }
        });

        app.MapHealthChecks();
        app.UseHealthChecksUI();

        app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandlingPath = "/error", });
        app.Map("/error", (HttpContext context) =>
        {
            var exception = context.GetFeature<IExceptionHandlerFeature>()?.Error;
            return Results.Problem(detail: exception?.Message);
        });

        app.UseMiddleware<CorrelationIdMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseDynamicAuthorization();

        app.MapCarter();

        return app;
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

    private static bool Is<T>(this AssemblyScanner.AssemblyScanResult result) => result.ValidatorType.IsAssignableTo(typeof(T));

    private static OptionsBuilder<TOptions> AddOptionsWithValidator<TOptions>(this IServiceCollection services, string? configSectionPath = null)
        where TOptions : class => services.AddOptions<TOptions>()
            .BindConfiguration(configSectionPath ?? typeof(TOptions).Name)
            .ValidateWithValidator()
            .ValidateOnStart();

    private static OptionsBuilder<TOptions> ValidateWithValidator<TOptions>(this OptionsBuilder<TOptions> builder, Action<ValidationStrategy<TOptions>>? validationOptions = null)
        where TOptions : class
    {
        var services = builder.Services;
        services.AddSingleton<IValidateOptions<TOptions>>(sp => new FluentOptionsValidator<TOptions>(builder.Name, sp.GetRequiredService<IValidator<TOptions>>(), validationOptions));
        return builder;
    }

    private sealed class FluentOptionsValidator<TOptions> : IValidateOptions<TOptions>
        where TOptions : class
    {
        private readonly Action<ValidationStrategy<TOptions>>? _validationOptions;

        public FluentOptionsValidator(string? name, IValidator<TOptions> validator, Action<ValidationStrategy<TOptions>>? validationOptions)
        {
            Name = name;
            Validator = validator;
            _validationOptions = validationOptions;
        }

        public string? Name { get; }

        public IValidator<TOptions> Validator { get; }

        public ValidateOptionsResult Validate(string? name, TOptions options)
        {
            // null name is used to configure all named options
            if (Name != null && name != Name)
            {
                // ignored if not validating this instance
                return ValidateOptionsResult.Skip;
            }

            var result = _validationOptions is null
                ? Validator.Validate(options)
                : Validator.Validate(options, _validationOptions);


            if (result.IsValid)
            {
                return ValidateOptionsResult.Success;
            }

            return ValidateOptionsResult.Fail(result.ToString());
        }
    }
}
