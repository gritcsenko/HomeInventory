using HomeInventory.Api.Common.Errors;
using HomeInventory.Application;
using HomeInventory.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddControllers(o =>
    {
        o.SuppressAsyncSuffixInActionNames = true;
    });
    builder.Services.AddSingleton<ProblemDetailsFactory, HomeInventoryProblemDetailsFactory>();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();

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
