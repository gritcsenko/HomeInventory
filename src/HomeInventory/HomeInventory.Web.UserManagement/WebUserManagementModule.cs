using Carter;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Modules.Interfaces;
using HomeInventory.Web.Framework;
using HomeInventory.Web.UserManagement.Authentication;
using HomeInventory.Web.UserManagement.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.UserManagement;

public sealed class WebUserManagementModule : BaseModuleWithCarter
{
    public override async Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken = default)
    {
        await base.AddServicesAsync(context, cancellationToken);

        context.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        context.Services
            .ConfigureOptions<JwtBearerOptionsSetup>()
            .AddSingleton<IJwtIdentityGenerator, UlidJwtIdentityGenerator>()
            .AddOptionsWithValidator<JwtOptions>();
        context.Services
            .AddScoped<IAuthenticationTokenGenerator, JwtAuthenticationTokenGenerator>();
    }

    public override void Configure(CarterConfigurator configurator)
    {
        AddValidatorsFromCurrentAssembly(configurator);
        AddCarterModulesFromCurrentAssembly(configurator);
    }
    
    public override async Task BuildAppAsync(IModuleBuildContext context, CancellationToken cancellationToken = default)
    {
        await base.BuildAppAsync(context, cancellationToken);

        context.ApplicationBuilder.UseAuthentication();
    }
    
}
