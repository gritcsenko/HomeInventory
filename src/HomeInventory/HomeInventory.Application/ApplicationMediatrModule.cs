using HomeInventory.Application.Cqrs.Behaviors;
using HomeInventory.Application.Framework;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application;

public sealed class ApplicationMediatrModule : BaseModuleWithMediatr
{
    public override void Configure(MediatRServiceConfiguration configuration)
    {
        configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
        configuration.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));

        base.Configure(configuration);
    }
}
