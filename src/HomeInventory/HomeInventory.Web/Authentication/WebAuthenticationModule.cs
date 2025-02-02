using Carter;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Modules.Interfaces;
using HomeInventory.Web.Configuration;
using HomeInventory.Web.Framework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.Authentication;

public sealed class WebAuthenticationModule : BaseModuleWithCarter
{
    public override async Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken = default)
    {
        await base.AddServicesAsync(context, cancellationToken);

        context.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        context.Services
            .ConfigureOptions<JwtBearerOptionsSetup>()
            .AddSingleton<IJwtIdentityGenerator, GuidJwtIdentityGenerator>()
            .AddOptionsWithValidator<JwtOptions>();
        context.Services
            .AddScoped<IAuthenticationTokenGenerator, JwtTokenGenerator>();
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
