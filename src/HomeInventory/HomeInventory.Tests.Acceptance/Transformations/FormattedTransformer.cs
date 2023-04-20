using HomeInventory.Tests.Acceptance.Support;

namespace HomeInventory.Tests.Acceptance.Transformations;

[Binding]
public class FormattedTransformer
{
    [StepArgumentTransformation]
    public DateOnly TransformDate(string dateText) => dateText.ParseDate();
}
