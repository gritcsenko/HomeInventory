using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Application.UserManagement.Services;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application.UserManagement;

public sealed class ApplicationUserManagementModule : BaseModule
{
    public override async Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken = default)
    {
        await base.AddServicesAsync(context, cancellationToken);

        context.Services
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<IRegistrationService, RegistrationService>();
    }
}
