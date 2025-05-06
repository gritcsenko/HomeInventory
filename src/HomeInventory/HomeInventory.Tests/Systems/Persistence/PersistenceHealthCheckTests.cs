using FluentAssertions.Execution;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute.ExceptionExtensions;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class PersistenceHealthCheckTests : BaseTest
{
    [Fact]
    public async Task CheckHealthAsync_ShouldReturnHealthy_WhenCanConnect()
    {
        var factory = new DbContextFactory(new SubstitutionDbContextFactory());
        var options = DbContextFactory.CreateInMemoryOptions<DatabaseContext>();
        var context = factory.CreateInMemory(DateTime, options);
        var database = Substitute.For<DatabaseFacade>(context);
        var providerName = Fixture.Create<string>();
        database.ProviderName.Returns(providerName);
        context.Database.Returns(database);
        database.CanConnectAsync(Cancellation.Token).Returns(true);
        var sut = new PersistenceHealthCheck(context);
        var healthContext = new HealthCheckContext
        {
            Registration = new(Fixture.Create<string>(), Substitute.For<IHealthCheck>(), HealthStatus.Degraded, []),
        };

        var result = await sut.CheckHealthAsync(healthContext, Cancellation.Token);

        using var scope = new AssertionScope();
        result.Status.Should().Be(HealthStatus.Healthy);
        result.Description.Should().Be("Database is healthy");
        result.Data.Should().Contain("provider", providerName);
    }

    [Fact]
    public async Task CheckHealthAsync_ShouldReturnDegraded_WhenCannotConnect()
    {
        var factory = new DbContextFactory(new SubstitutionDbContextFactory());
        var context = factory.CreateInMemory<DatabaseContext>(DateTime);
        var database = Substitute.For<DatabaseFacade>(context);
        var providerName = Fixture.Create<string>();
        var failureStatus = HealthStatus.Degraded;
        database.ProviderName.Returns(providerName);
        context.Database.Returns(database);
        database.CanConnectAsync(Cancellation.Token).Returns(false);
        var sut = new PersistenceHealthCheck(context);
        var healthContext = new HealthCheckContext
        {
            Registration = new(Fixture.Create<string>(), Substitute.For<IHealthCheck>(), failureStatus, []),
        };

        var result = await sut.CheckHealthAsync(healthContext, Cancellation.Token);

        using var scope = new AssertionScope();
        result.Status.Should().Be(failureStatus);
        result.Description.Should().Be("Cannot connect to the database");
        result.Data.Should().Contain("provider", providerName);
    }

    [Fact]
    public async Task CheckHealthAsync_ShouldReturnDegraded_WhenThrows()
    {
        var factory = new DbContextFactory(new SubstitutionDbContextFactory());
        var context = factory.CreateInMemory<DatabaseContext>(DateTime);
        var database = Substitute.For<DatabaseFacade>(context);
        var providerName = Fixture.Create<string>();
        var failureStatus = HealthStatus.Degraded;
        database.ProviderName.Returns(providerName);
        context.Database.Returns(database);
        database.CanConnectAsync(Cancellation.Token).ThrowsAsync<InvalidOperationException>();
        var sut = new PersistenceHealthCheck(context);
        var healthContext = new HealthCheckContext
        {
            Registration = new(Fixture.Create<string>(), Substitute.For<IHealthCheck>(), failureStatus, []),
        };

        var result = await sut.CheckHealthAsync(healthContext, Cancellation.Token);

        using var scope = new AssertionScope();
        result.Status.Should().Be(failureStatus);
        result.Description.Should().Be("Failed to perform healthcheck");
        result.Data.Should().Contain("provider", providerName);
        result.Exception.Should().BeOfType<InvalidOperationException>();
    }
}
