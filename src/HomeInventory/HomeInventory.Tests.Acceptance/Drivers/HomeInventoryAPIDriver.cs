using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Tests.Framework;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal sealed class HomeInventoryApiDriver : WebApplicationFactory<Api.AppBuilder>, IHomeInventoryApiDriver
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
        Services.GetRequiredService<MutableDateTimeService>().SetUtcNow(today.ToDateTime(new(12, 0, 0)));

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_configuration.EnvironmentName);

        // Add mock/test services to the builder here
        builder.ConfigureServices(services =>
        {
            var id = Ulid.NewUlid();
            // Replace real database with in-memory database for tests
            services.ReplaceWithSingleton(_ => DbContextFactory.CreateInMemoryOptions<DatabaseContext>("HomeInventory", id));
            services.AddSingleton<MutableDateTimeService>();
            services.ReplaceWithScoped<TimeProvider>(sp => sp.GetRequiredService<MutableDateTimeService>());
        });

        return base.CreateHost(builder);
    }

    private UserManagementApiDriver CreateUserManagement() => new(Server);

    private sealed class MutableDateTimeService : TimeProvider
    {
        private DateTimeOffset _utcNow = DateTimeOffset.UtcNow;

        public override DateTimeOffset GetUtcNow() => _utcNow;

        public void SetUtcNow(DateTimeOffset value) => _utcNow = value;
    }
}
