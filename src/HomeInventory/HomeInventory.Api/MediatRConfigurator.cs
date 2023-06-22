using HomeInventory.Application;
using MediatR.NotificationPublishers;

namespace HomeInventory.Api;

internal static class MediatRConfigurator
{
    public static void Configure(MediatRServiceConfiguration configuration) =>
        configuration
            .RegisterServicesFromAssemblies(Application.AssemblyReference.Assembly)
            .AddLoggingBehavior()
            .AddUnitOfWorkBehavior()
            .SetNotificationPublisher<TaskWhenAllPublisher>();
}
