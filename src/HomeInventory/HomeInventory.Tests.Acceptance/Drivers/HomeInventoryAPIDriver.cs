﻿using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Tests.Support;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Tests.Acceptance.Drivers;
internal class HomeInventoryAPIDriver : WebApplicationFactory<Program>, IHomeInventoryAPIDriver
{
    private readonly ITestingConfiguration _configuration;
    private readonly Lazy<IAuthenticationAPIDriver> _lazyAuthentication;

    public HomeInventoryAPIDriver(ITestingConfiguration configuration)
    {
        _configuration = configuration;
        _lazyAuthentication = new(CreateAuthentication, true);
    }
    public IAuthenticationAPIDriver Authentication => _lazyAuthentication.Value;

    public void SetToday(DateOnly today) =>
        Services.GetRequiredService<FixedTestingDateTimeService>().Now = today.ToDateTime(new TimeOnly(12, 0, 0));

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_configuration.EnvironmentName);

        // Add mock/test services to the builder here
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<DatabaseContext>>();
            var id = Guid.NewGuid();
            // Replace real database with in-memory database for tests
            services.AddScoped(sp => new DbContextOptionsBuilder<DatabaseContext>()
                .UseApplicationServiceProvider(sp)
                .UseInMemoryDatabase($"HomeInventory{id:D}")
                .Options);
            services.AddSingleton<FixedTestingDateTimeService>();
            services.AddSingleton<IDateTimeService>(sp => sp.GetRequiredService<FixedTestingDateTimeService>());
        });

        return base.CreateHost(builder);
    }

    private AuthenticationAPIDriver CreateAuthentication() => new(Server);
}
