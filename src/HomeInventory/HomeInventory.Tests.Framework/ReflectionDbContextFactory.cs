using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using HomeInventory.Tests.Framework;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests;

internal sealed class ReflectionDbContextFactory : IDbContextFactory
{
    public TContext Create<TContext>(DbContextOptions<TContext> options, PublishDomainEventsInterceptor interceptor, IDateTimeService dateTimeService, IDatabaseConfigurationApplier[] appliers)
        where TContext : DbContext =>
        ReflectionMethods.CreateInstance<TContext>(options, interceptor, dateTimeService, appliers)
            ?? throw new InvalidOperationException($"Failed to create {typeof(TContext).AssemblyQualifiedName}");
}
