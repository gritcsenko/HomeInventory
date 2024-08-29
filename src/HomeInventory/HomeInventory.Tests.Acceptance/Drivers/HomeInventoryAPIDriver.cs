﻿using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Tests.Framework;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal sealed class HomeInventoryApiDriver : WebApplicationFactory<Program>, IHomeInventoryApiDriver
{
    private readonly ITestingConfiguration _configuration;
    private readonly Lazy<IAuthenticationApiDriver> _lazyAuthentication;
    private readonly Lazy<IUserManagementApiDriver> _lazyUserManagement;
    private readonly Lazy<IAreaApiDriver> _lazyArea;

    public HomeInventoryApiDriver(ITestingConfiguration configuration)
    {
        _configuration = configuration;
        _lazyAuthentication = new(CreateAuthentication, true);
        _lazyUserManagement = new(CreateUserManagement, true);
        _lazyArea = new(CreateArea, true);
    }

    public IAuthenticationApiDriver Authentication => _lazyAuthentication.Value;

    public IUserManagementApiDriver UserManagement => _lazyUserManagement.Value;

    public IAreaApiDriver Area => _lazyArea.Value;

    public void SetToday(DateOnly today) =>
        Services.GetRequiredService<MutableDateTimeService>().SetUtcNow(today.ToDateTime(new TimeOnly(12, 0, 0)));

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_configuration.EnvironmentName);

        // Add mock/test services to the builder here
        builder.ConfigureServices(services =>
        {
            // Replace real database with in-memory database for tests
            services.ReplaceWithSingleton(sp => DbContextFactory.CreateInMemoryOptions<DatabaseContext>("HomeInventory"));
            services.AddSingleton<MutableDateTimeService>();
            services.ReplaceWithScoped<TimeProvider>(sp => sp.GetRequiredService<MutableDateTimeService>());
        });

        return base.CreateHost(builder);
    }

    private AuthenticationApiDriver CreateAuthentication() => new(Server);

    private UserManagementApiDriver CreateUserManagement() => new(Server);

    private AreaApiDriver CreateArea() => new(Server);

    private sealed class MutableDateTimeService : TimeProvider
    {
        private DateTimeOffset _utcNow = DateTimeOffset.UtcNow;

        public override DateTimeOffset GetUtcNow() => _utcNow;

        public void SetUtcNow(DateTimeOffset value) => _utcNow = value;
    }
}
