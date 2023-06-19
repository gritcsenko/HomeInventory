using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal sealed class HomeInventoryApiDriver : WebApplicationFactory<Program>, IHomeInventoryApiDriver
{
    private readonly ITestingConfiguration _configuration;
    private readonly Lazy<IUserManagementApiDriver> _lazyUserManagement;

    public HomeInventoryApiDriver(ITestingConfiguration configuration)
    {
        _configuration = configuration;
        _lazyUserManagement = new(CreateUserManagement, true);
    }

    public IUserManagementApiDriver UserManagement => _lazyUserManagement.Value;

    public void SetToday(DateOnly today) =>
        Services.GetRequiredService<MutableDateTimeService>().UtcNow = today.ToDateTime(new TimeOnly(12, 0, 0));

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_configuration.EnvironmentName);

        // Add mock/test services to the builder here
        builder.ConfigureServices(services =>
        {
            var id = Ulid.NewUlid();
            // Replace real database with in-memory database for tests
            services.ReplaceWithSingleton(sp => new DbContextOptionsBuilder<DatabaseContext>()
                .UseApplicationServiceProvider(sp)
                .UseInMemoryDatabase($"HomeInventory{id}")
                .Options);
            services.AddSingleton<MutableDateTimeService>();
            services.ReplaceWithScoped<IDateTimeService>(sp => sp.GetRequiredService<MutableDateTimeService>());
        });

        return base.CreateHost(builder);
    }

    private UserManagementApiDriver CreateUserManagement() => new(Server);

    private sealed class MutableDateTimeService : IDateTimeService
    {
        public DateTimeOffset UtcNow { get; set; } = DateTimeOffset.UtcNow;
    }
}
