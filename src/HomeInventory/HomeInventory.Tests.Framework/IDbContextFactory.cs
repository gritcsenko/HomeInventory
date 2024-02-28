using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests;

internal interface IDbContextFactory
{
    TContext Create<TContext>(DbContextOptions<TContext> options, PublishDomainEventsInterceptor interceptor, IDateTimeService dateTimeService, IDatabaseConfigurationApplier[] appliers) where TContext : DbContext;
}
