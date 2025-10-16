using System.Globalization;
using HomeInventory.Tests.Acceptance.Support;

namespace HomeInventory.Tests.Acceptance.Transformations;

[Binding]
public sealed class FormattedTransformer
{
    private readonly IFormatProvider _formatProvider = CultureInfo.CurrentCulture;

    [StepArgumentTransformation(Patterns.DateOnly, Name = nameof(Patterns.DateOnly))]
    public DateOnly TransformDate(string dateText) => dateText.ParseDate(formatProvider: _formatProvider);
}
