using HomeInventory.Domain.Events;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using HomeInventory.Infrastructure.UserManagement.Models.Configurations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Framework;

public class DbContextFactory
{
    private readonly IDbContextFactory _factory;

    internal DbContextFactory(IDbContextFactory factory)
    {
        _factory = factory;
    }

    public static DbContextFactory Default { get; } = new DbContextFactory(new ReflectionDbContextFactory());

    public TContext CreateInMemory<TContext>(TimeProvider dateTimeService)
        where TContext : DbContext
    {
        var options = CreateInMemoryOptions<TContext>();
        return CreateInMemory(dateTimeService, options);
    }

    public TContext CreateInMemory<TContext>(TimeProvider dateTimeService, DbContextOptions<TContext> options)
        where TContext : DbContext =>
        CreateInMemory(
            dateTimeService,
            options,
            new OutboxDatabaseConfigurationApplier(new PolymorphicDomainEventTypeResolver([new DomainEventJsonTypeInfo(typeof(DomainEvent), typeof(UserCreatedDomainEvent))])),
            new UserModelDatabaseConfigurationApplier());

    public TContext CreateInMemory<TContext>(TimeProvider dateTimeService, DbContextOptions<TContext> options, params IDatabaseConfigurationApplier[] appliers)
        where TContext : DbContext
    {
        var interceptor = new PublishDomainEventsInterceptor(Substitute.For<IPublisher>());
        return _factory.Create(options, interceptor, dateTimeService, appliers);
    }

    public static DbContextOptions<TContext> CreateInMemoryOptions<TContext>(string dbNamePrefix = "db", Ulid? id = null)
        where TContext : DbContext =>
        new DbContextOptionsBuilder<TContext>()
            .UseInMemoryDatabase(databaseName: dbNamePrefix + (id ?? IdSuppliers.Ulid.Invoke()))
            .EnableSensitiveDataLogging()
            .Options;
}
