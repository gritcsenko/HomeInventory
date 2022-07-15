using HomeInventory.Api;
using HomeInventory.Application;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using Mapster;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    builder.Services.AddDomain();
    builder.Services.AddApi();
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = (ctx, report) => ctx.Response.WriteAsJsonAsync(report)
    });
    app.UseHealthChecksUI();
    app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = _ => true/*healthCheck => healthCheck.Tags.Contains("ready")*/ });
    app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });

    app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandlingPath = "/error", });
    app.Map("/error", (HttpContext context) =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        return Results.Problem(detail: exception?.Message);
    });
    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
