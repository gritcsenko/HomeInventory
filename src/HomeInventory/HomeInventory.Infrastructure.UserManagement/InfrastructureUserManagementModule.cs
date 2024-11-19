using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Domain.UserManagement.Events;
using HomeInventory.Infrastructure.Framework;
using HomeInventory.Infrastructure.UserManagement.Models.Configurations;
using HomeInventory.Infrastructure.UserManagement.Services;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Infrastructure.UserManagement;

public sealed class InfrastructureUserManagementModule : BaseModule
{
    public override async Task AddServicesAsync(IModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        context.Services
            .AddSingleton<IPasswordHasher, BCryptPasswordHasher>()
            .AddSingleton<IJsonDerivedTypeInfo>(static _ => new DomainEventJsonTypeInfo(typeof(UserCreatedDomainEvent)));
    }
}
