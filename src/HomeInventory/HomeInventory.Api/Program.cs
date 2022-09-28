using HomeInventory.Application;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Web;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

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

public partial class Program { }
