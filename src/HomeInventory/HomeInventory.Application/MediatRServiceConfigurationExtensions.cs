﻿using HomeInventory.Application.Cqrs.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application;

public static class MediatRServiceConfigurationExtensions
{
    public static MediatRServiceConfiguration AddLoggingBehavior(this MediatRServiceConfiguration configuration) =>
        configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));

    public static MediatRServiceConfiguration AddUnitOfWorkBehavior(this MediatRServiceConfiguration configuration) =>
        configuration.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
}
