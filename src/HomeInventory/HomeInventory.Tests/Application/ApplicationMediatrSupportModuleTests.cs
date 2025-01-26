using System.Diagnostics.CodeAnalysis;
using HomeInventory.Application.Framework;
using HomeInventory.Modules.Interfaces;
using HomeInventory.Tests.Modules;
using MediatR;
using MediatR.NotificationPublishers;

namespace HomeInventory.Tests.Application;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public sealed class ApplicationMediatrSupportModuleTests() : BaseModuleTest<ApplicationMediatrSupportModule>(static () => new())
{
    private readonly SubjectBaseModuleWithMediatr _subject = new();

    protected override IReadOnlyCollection<IModule> GetModules() => [_subject];

    protected override void EnsureRegistered(IServiceCollection services)
    {
        _subject.Configured.Should().BeTrue();
        services.Should()
            .ContainSingleTransient<IMediator>()
            .And.ContainSingleTransient<ISender>()
            .And.ContainSingleTransient<IPublisher>()
            .And.ContainSingleTransient<INotificationPublisher>()
            .And.ContainSingleSingleton<MediatRServiceConfiguration>()
            .Which.ImplementationInstance.Should().BeOfType<MediatRServiceConfiguration>()
            .Which.NotificationPublisherType.Should().Be<TaskWhenAllPublisher>();
    }
}
