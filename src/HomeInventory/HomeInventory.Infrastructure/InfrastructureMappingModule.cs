﻿using HomeInventory.Application.Framework;
using HomeInventory.Domain;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Infrastructure;

public sealed class InfrastructureMappingModule : BaseModule
{
    public InfrastructureMappingModule()
    {
        DependsOn<DomainModule>();
        DependsOn<ApplicationMappingModule>();
    }

    public override async Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken = default)
    {
        await base.AddServicesAsync(context, cancellationToken);

        context.Services
            .AddSingleton<AmountObjectConverter>()
            .AddMappingAssemblySource(GetType().Assembly);
    }
}
