using HomeInventory.Infrastructure;
using HomeInventory.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWeb();
builder.Services.AddInfrastructure();

var app = builder.Build();
app.UseWeb();
app.Run();
