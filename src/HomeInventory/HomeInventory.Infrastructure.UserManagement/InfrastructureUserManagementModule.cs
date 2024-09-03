using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Events;
using HomeInventory.Infrastructure.Framework;
using HomeInventory.Infrastructure.Services;
using HomeInventory.Infrastructure.UserManagement.Models.Configurations;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public sealed class InfrastructureUserManagementModule : BaseModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IJsonDerivedTypeInfo>(_ => new DomainEventJsonTypeInfo(typeof(UserCreatedDomainEvent)));
    }
}
