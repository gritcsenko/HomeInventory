using Carter;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Web.Configuration;
using HomeInventory.Web.Framework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.Authentication;

public sealed class WebAuthenticationModule : BaseModuleWithCarter
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        base.AddServices(services, configuration);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddSingleton<IJwtIdentityGenerator, GuidJwtIdentityGenerator>();
        services.AddOptionsWithValidator<JwtOptions>();
        services.AddScoped<IAuthenticationTokenGenerator, JwtTokenGenerator>();
    }

    public override void Configure(CarterConfigurator configurator)
    {
        AddValidatorsFromCurrentAssembly(configurator);
        AddCarterModulesFromCurrentAssembly(configurator);
    }

    public override void BuildApp(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder)
    {
        applicationBuilder.UseAuthentication();
    }
}
