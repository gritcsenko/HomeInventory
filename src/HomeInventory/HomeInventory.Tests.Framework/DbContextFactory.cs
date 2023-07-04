using HomeInventory.Core;
using HomeInventory.Domain.Events;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using HomeInventory.Infrastructure.UserManagement.Models.Configurations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests;

public static class DbContextFactory
{
    public static TContext CreateInMemory<TContext>(IDateTimeService dateTimeService)
        where TContext : DbContext
    {
        var options = CreateInMemoryOptions<TContext>();
        return CreateInMemory(dateTimeService, options);
    }

    public static TContext CreateInMemory<TContext>(IDateTimeService dateTimeService, DbContextOptions<TContext> options)
        where TContext : DbContext =>
        CreateInMemory(
            dateTimeService,
            options,
            new OutboxDatabaseConfigurationApplier(new PolymorphicDomainEventTypeResolver(new[] { new DomainEventJsonTypeInfo(typeof(DomainEvent), typeof(UserCreatedDomainEvent)) })),
            new UserModelDatabaseConfigurationApplier());

    public static TContext CreateInMemory<TContext>(IDateTimeService dateTimeService, DbContextOptions<TContext> options, params IDatabaseConfigurationApplier[] appliers)
        where TContext : DbContext
    {
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
