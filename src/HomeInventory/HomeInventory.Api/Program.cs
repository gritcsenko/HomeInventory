using HomeInventory.Infrastructure;
using HomeInventory.Web;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HomeInventory.Tests")]

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    builder.Services.AddWeb();
    builder.Services.AddInfrastructure(builder.Configuration);
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    app.UseWeb();

    app.Run();
}
