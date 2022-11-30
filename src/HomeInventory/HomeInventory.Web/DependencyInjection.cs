using System.Reflection;
using System.Security.Claims;
using FluentValidation;
using FluentValidation.AspNetCore;
using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Web.Authentication;
using HomeInventory.Web.Configuration;
using HomeInventory.Web.Configuration.Interfaces;
using HomeInventory.Web.Infrastructure;
using HomeInventory.Web.Middleware;
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

        services.AddSingleton<PermissionList>();
        services.AddSingleton<IAuthorizationHandler, DynamicAuthorizationHandler>();

        services.AddAuthorization(b =>
        {
            b.AddPolicy("dynamic", pb => pb
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .AddRequirements(new DynamicRequirement()));
        }); // Read https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-6.0

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddSingleton<IJwtIdentityGenerator, GuidJwtIdentityGenerator>();
        services.AddSingleton<IAuthenticationTokenGenerator, JwtTokenGenerator>();

        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        services.ConfigureOptions<SwaggerGenOptionsSetup>();

        services.AddFluentValidationAutoValidation(c =>
        {
            c.DisableDataAnnotationsValidation = true;
        });
        services.AddValidatorsFromAssembly(Contracts.Validations.AssemblyReference.Assembly, includeInternalTypes: true);

        return services;
    }

    public static T UseWeb<T>(this T app)
        where T : IApplicationBuilder, IEndpointRouteBuilder
    {
        app.UseSwagger();
        app.UseSwaggerUI();

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

        app.UseAreaEndpoints();
        app.CollectEnpointTags(app.ApplicationServices.GetRequiredService<PermissionList>());

        app.UseMiddleware<CorrelationIdMiddleware>();

        return app;
    }

    private static IEndpointRouteBuilder UseAreaEndpoints(this IEndpointRouteBuilder builder)
    {
        var areas = builder.MapGroup("/api/areas");
        areas.MapGet("", () => { }).RequireAuthorization("dynamic").WithTags("auth:get-areas", "auth:areas");

        return builder;
    }

    private static IEndpointRouteBuilder CollectEnpointTags(this IEndpointRouteBuilder builder, PermissionList permissionList)
    {
        var datasource = builder.DataSources.First();
        foreach (var endpoint in datasource.Endpoints)
        {
            var tags = endpoint.GetAuthTags();

            foreach (var tag in tags)
            {
                permissionList.Add(tag);
            }
        }

        return builder;
    }

    private static IEnumerable<string> GetAuthTags(this Endpoint endpoint)
    {
        return endpoint.Metadata
                .OfType<TagsAttribute>()
                .SelectMany(a => a.Tags)
                .Where(a => a?.StartsWith("auth") == true);
    }

    private class DynamicAuthorizationHandler : AuthorizationHandler<DynamicRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicRequirement requirement)
        {
            if (context.Resource is not HttpContext httpContext)
            {
                context.Fail(new AuthorizationFailureReason(this, "Context has invalid resource"));
                return;
            }

            if (httpContext.GetEndpoint() is not Endpoint endpoint)
            {
                context.Fail(new AuthorizationFailureReason(this, "HTTP context has ho endpoint"));
                return;
            }


            if (context.User.FindFirstValue(ClaimTypes.NameIdentifier) is not string idText)
            {
                context.Fail(new AuthorizationFailureReason(this, "User has no id"));
                return;
            }

            using var scope = httpContext.RequestServices.CreateScope();

            var userIdFactory = scope.ServiceProvider.GetRequiredService<IIdFactory<UserId, string>>();
            var userIdResult = userIdFactory.CreateFrom(idText);
            if (userIdResult.IsFailed)
            {
                context.Fail(new AuthorizationFailureReason(this, $"User has invalid id: {string.Join(';', userIdResult.Reasons.Select(r => r.Message))}"));
                return;
            }
            var userId = userIdResult.Value;

            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var tags = endpoint.GetAuthTags();
            foreach (var tag in tags)
            {
                if (await repository.HasPermissionAsync(userId, tag, httpContext.RequestAborted))
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            context.Fail(new AuthorizationFailureReason(this, $"User has no permission"));
        }
    }

    private class DynamicRequirement : IAuthorizationRequirement
    {
    }

    private class PermissionList : HashSet<string>
    {
    }
}
