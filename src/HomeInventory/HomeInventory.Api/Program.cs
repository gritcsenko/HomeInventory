using HomeInventory.Application;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDomain();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddWeb();

var app = builder.Build();
app.UseWeb();
app.Run();
