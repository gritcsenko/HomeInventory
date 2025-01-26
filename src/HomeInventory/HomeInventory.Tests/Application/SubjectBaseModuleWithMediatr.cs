using HomeInventory.Application.Framework;

namespace HomeInventory.Tests.Application;

public sealed class SubjectBaseModuleWithMediatr : BaseModuleWithMediatr
{
    public bool Configured { get; private set; }

    public override void Configure(MediatRServiceConfiguration configuration) => Configured = true;
}
