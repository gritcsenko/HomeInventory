using HomeInventory.Core;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests;

public static class DbContextFactory
{
    public static TContext CreateInMemory<TContext>(IDateTimeService dateTimeService, params IDatabaseConfigurationApplier[] appliers)
        where TContext : DbContext
    {
        var options = CreateInMemoryOptions<TContext>();
        var interceptor = new PublishDomainEventsInterceptor(Substitute.For<IPublisher>());
        return ReflectionMethods.CreateInstance<TContext>(options, interceptor, dateTimeService, appliers)
            ?? throw new InvalidOperationException($"Failed to create {typeof(TContext).AssemblyQualifiedName}");
    }

    public static DbContextOptions<TContext> CreateInMemoryOptions<TContext>(string dbNamePrefix = "db", Ulid? id = null)
        where TContext : DbContext =>
        new DbContextOptionsBuilder<TContext>()
            .UseInMemoryDatabase(databaseName: dbNamePrefix + (id ?? Ulid.NewUlid()))
            .EnableSensitiveDataLogging()
            .Options;
}
