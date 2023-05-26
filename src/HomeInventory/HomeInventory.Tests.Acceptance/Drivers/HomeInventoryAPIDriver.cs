using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal sealed class HomeInventoryAPIDriver : WebApplicationFactory<Program>, IHomeInventoryAPIDriver
{
    private readonly ITestingConfiguration _configuration;
    private readonly Lazy<IUserManagementAPIDriver> _lazyUserManagement;

    public HomeInventoryAPIDriver(ITestingConfiguration configuration)
    {
        _configuration = configuration;
        _lazyUserManagement = new(CreateUserManagement, true);
    }

    public IUserManagementAPIDriver UserManagement => _lazyUserManagement.Value;

    public void SetToday(DateOnly today) =>
        Services.GetRequiredService<MutableDateTimeService>().UtcNow = today.ToDateTime(new TimeOnly(12, 0, 0));

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_configuration.EnvironmentName);

        // Add mock/test services to the builder here
        builder.ConfigureServices(services =>
        {
            var id = Guid.NewGuid();
            // Replace real database with in-memory database for tests
            services.ReplaceWithSingleton(sp => new DbContextOptionsBuilder<DatabaseContext>()
                .UseApplicationServiceProvider(sp)
                .UseInMemoryDatabase($"HomeInventory{id:D}")
                .Options);
            services.AddSingleton<MutableDateTimeService>();
            services.ReplaceWithScoped<IDateTimeService>(sp => sp.GetRequiredService<MutableDateTimeService>());
        });

        var host = base.CreateHost(builder);
        var server = host.Services.GetRequiredService<IServer>();
        var serverAddresses = server.Features.Get<IServerAddressesFeature>();
        serverAddresses?.Addresses.Add("http://localhost:5006");
        return host;
    }

    private UserManagementAPIDriver CreateUserManagement() => new(Server);

    private sealed class MutableDateTimeService : IDateTimeService
    {
        public DateTimeOffset UtcNow { get; set; } = DateTimeOffset.UtcNow;
    }
}
