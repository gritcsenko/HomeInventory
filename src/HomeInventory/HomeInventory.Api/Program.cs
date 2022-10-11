using HomeInventory.Application;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Web;
using MediatR;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog(ConfigureSerilog, preserveStaticLogger: false, writeToProviders: false);
    builder.Services.AddMediatR(HomeInventory.Application.AssemblyReference.Assembly, HomeInventory.Infrastructure.AssemblyReference.Assembly);

    builder.Services.AddDomain();
    builder.Services.AddInfrastructure();
    builder.Services.AddApplication();
    builder.Services.AddWeb();

    var app = builder.Build();
    app.UseWeb();

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
}
finally
{
    Log.CloseAndFlush();
}

static void ConfigureSerilog(HostBuilderContext context, IServiceProvider services, LoggerConfiguration configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);

public partial class Program { }
