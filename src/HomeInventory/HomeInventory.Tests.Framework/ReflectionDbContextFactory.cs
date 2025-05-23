﻿using HomeInventory.Infrastructure.Framework.Models.Configuration;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Framework;

internal sealed class ReflectionDbContextFactory : IDbContextFactory
{
    public TContext Create<TContext>(DbContextOptions<TContext> options, PublishDomainEventsInterceptor interceptor, TimeProvider dateTimeService, IDatabaseConfigurationApplier[] appliers)
        where TContext : DbContext =>
        ReflectionMethods.CreateInstance<TContext>(options, interceptor, dateTimeService, appliers)
            ?? throw new InvalidOperationException($"Failed to create {typeof(TContext).AssemblyQualifiedName}");
}
