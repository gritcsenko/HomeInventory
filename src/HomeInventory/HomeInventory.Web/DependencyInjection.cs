﻿using System.Reflection;
using Asp.Versioning;
using Carter;
using FluentValidation;
using FluentValidation.AspNetCore;
using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Web.Authentication;
using HomeInventory.Web.Authorization.Dynamic;
using HomeInventory.Web.Configuration;
using HomeInventory.Web.Configuration.Interfaces;
using HomeInventory.Web.Infrastructure;
using HomeInventory.Web.Middleware;
using HomeInventory.Web.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health
        // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0
        services.AddHealthChecks();
        services.AddHealthChecksUI()
            .AddInMemoryStorage();

        services.AddSingleton<ProblemDetailsFactory, HomeInventoryProblemDetailsFactory>();
        services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
        services.AddScoped<CorrelationIdMiddleware>();

        services.AddAutoMapper((sp, configExpression) =>
        {
            configExpression.AddMaps(sp.GetServices<IMappingAssemblySource>().Select(s => s.GetAssembly()));
            configExpression.ConstructServicesUsing(sp.GetService);
        }, Type.EmptyTypes);

        services.AddMappingAssemblySource(AssemblyReference.Assembly);

        services.AddControllers(o => o.SuppressAsyncSuffixInActionNames = true)
            .AddApplicationPart(Assembly.GetExecutingAssembly())
            .AddControllersAsServices();

        services.AddDynamicAuthorization();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.ConfigureOptions<ConfigureSwaggerOptions>();
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
        services.AddSingleton<IAuthenticationTokenGenerator, JwtTokenGenerator>();

        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddFluentValidationAutoValidation(c =>
        {
            c.DisableDataAnnotationsValidation = true;
        });
        services.AddValidatorsFromAssembly(Contracts.Validations.AssemblyReference.Assembly, includeInternalTypes: true);

        services.AddCarter(configurator: config => config
            .WithModule<AreaModule>()
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
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);
            }
        });

        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = (ctx, report) => ctx.Response.WriteAsJsonAsync(report)
        });
        app.UseHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = x => x.Tags.Contains("ready"),
            ResponseWriter = (ctx, report) => ctx.Response.WriteAsJsonAsync(report)
        });
        app.UseHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = (ctx, report) => ctx.Response.WriteAsJsonAsync(report)
        });
        app.UseHealthChecksUI();

        app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandlingPath = "/error", });
        app.Map("/error", (HttpContext context) =>
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            return Results.Problem(detail: exception?.Message);
        });

        app.UseMiddleware<CorrelationIdMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseDynamicAuthorization();

        app.MapCarter();

        return app;
    }

    private static TBuilder RequirePermission<TBuilder>(this TBuilder builder, params Permission[] permissions)
        where TBuilder : IEndpointConventionBuilder
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var attributes = permissions.Select(p => new AuthorizeAttribute(policy: p.ToString())).ToArray();

        return builder.RequireAuthorization(attributes);
    }
}
