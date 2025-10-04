using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Tests.Application;

public sealed class FeatureFlagGivenTestContext(BaseTest test) : GivenContext<FeatureFlagGivenTestContext>(test)
{
    internal FeatureFlagGivenTestContext Sut(out IVariable<IFeatureFlag> sut, IVariable<string> nameVariable) =>
        New(out sut, () => Create(nameVariable));

    internal FeatureFlagGivenTestContext Sut(out IVariable<IFeatureFlag<Guid>> sut, IVariable<string> nameVariable, IVariable<Guid> contextVariable) =>
        New(out sut, () => Create(nameVariable, contextVariable));

    private IFeatureFlag Create(IVariable<string> nameVariable) =>
        FeatureFlag.Create(GetValue(nameVariable));

    private IFeatureFlag<TContext> Create<TContext>(IVariable<string> nameVariable, IVariable<TContext> contextVariable)
        where TContext : notnull =>
        FeatureFlag.Create(GetValue(nameVariable), GetValue(contextVariable));
}
