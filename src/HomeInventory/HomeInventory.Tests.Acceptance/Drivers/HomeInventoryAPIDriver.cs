using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Tests.Acceptance.Drivers;
internal class HomeInventoryAPIDriver : WebApplicationFactory<Program>, IHomeInventoryAPIDriver
{
    private readonly ITestingConfiguration _configuration;
    private readonly Lazy<IAuthenticationAPIDriver> _lazyAuthentication;
    private readonly Lazy<IAreaAPIDriver> _lazyArea;

    public HomeInventoryAPIDriver(ITestingConfiguration configuration)
    {
        _configuration = configuration;
        _lazyAuthentication = new(CreateAuthentication, true);
        _lazyArea = new(CreateArea, true);
    }

    public IAuthenticationAPIDriver Authentication => _lazyAuthentication.Value;

    public IAreaAPIDriver Area => _lazyArea.Value;

    public void SetToday(DateOnly today) =>
        Services.GetRequiredService<FixedTestingDateTimeService>().Now = today.ToDateTime(new TimeOnly(12, 0, 0));

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_configuration.EnvironmentName);

        // Add mock/test services to the builder here
        builder.ConfigureServices(services =>
        {
            var id = Guid.NewGuid();
            // Replace real database with in-memory database for tests
            services.ReplaceWithScoped(sp => new DbContextOptionsBuilder<DatabaseContext>()
                .UseApplicationServiceProvider(sp)
                .UseInMemoryDatabase($"HomeInventory{id:D}")
                .Options);
            services.AddSingleton<FixedTestingDateTimeService>();
            services.ReplaceWithSingleton<IDateTimeService>(sp => sp.GetRequiredService<FixedTestingDateTimeService>());
        });

        return base.CreateHost(builder);
    }

    private AuthenticationAPIDriver CreateAuthentication() => new(Server);

    private AreaAPIDriver CreateArea() => new(Server);
}
