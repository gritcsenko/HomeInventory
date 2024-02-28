﻿using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests;

internal sealed class SubstitutionDbContextFactory : IDbContextFactory
{
    public TContext Create<TContext>(DbContextOptions<TContext> options, PublishDomainEventsInterceptor interceptor, IDateTimeService dateTimeService, IDatabaseConfigurationApplier[] appliers)
        where TContext : DbContext =>
        Substitute.For<TContext>(options, interceptor, dateTimeService, appliers);
}
