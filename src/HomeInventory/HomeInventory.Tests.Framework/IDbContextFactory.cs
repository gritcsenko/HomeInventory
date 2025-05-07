using HomeInventory.Infrastructure.Framework.Models.Configuration;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Framework;

internal interface IDbContextFactory
{
    TContext Create<TContext>(DbContextOptions<TContext> options, PublishDomainEventsInterceptor interceptor, TimeProvider dateTimeService, IDatabaseConfigurationApplier[] appliers) where TContext : DbContext;
}
