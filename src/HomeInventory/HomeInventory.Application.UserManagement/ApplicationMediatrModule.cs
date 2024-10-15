using HomeInventory.Application.Framework;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application.UserManagement;

public sealed class ApplicationUserManagementMediatrModule : BaseModuleWithMediatr
{
    public override void Configure(MediatRServiceConfiguration configuration)
    {
        RegisterServicesFromCurrentAssembly(configuration);
    }
}
