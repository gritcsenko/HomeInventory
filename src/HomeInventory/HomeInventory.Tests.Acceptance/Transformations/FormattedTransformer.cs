using HomeInventory.Tests.Acceptance.Support;

namespace HomeInventory.Tests.Acceptance.Transformations;

[Binding]
public sealed class FormattedTransformer
{
    [StepArgumentTransformation]
#pragma warning disable CA1822 // Mark members as static
    public DateOnly TransformDate(string dateText) => dateText.ParseDate();
#pragma warning restore CA1822 // Mark members as static
}
