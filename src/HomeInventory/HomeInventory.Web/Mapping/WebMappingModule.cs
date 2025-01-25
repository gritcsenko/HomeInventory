﻿using HomeInventory.Application.Framework;
using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Web.Mapping;

public sealed class WebMappingModule : BaseModule
{
    public WebMappingModule() => DependsOn<ApplicationMappingModule>();

    public override async Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken = default)
    {
        await base.AddServicesAsync(context, cancellationToken);

        context.Services
            .AddMappingAssemblySource(GetType().Assembly);
    }
}
