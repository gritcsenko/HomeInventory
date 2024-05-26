using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Framework;

internal sealed class SubstitutionDbContextFactory : IDbContextFactory
{
    public TContext Create<TContext>(DbContextOptions<TContext> options, PublishDomainEventsInterceptor interceptor, TimeProvider dateTimeService, IDatabaseConfigurationApplier[] appliers)
        where TContext : DbContext =>
        Substitute.For<TContext>(options, interceptor, dateTimeService, appliers);
}
