using HomeInventory.Web.Framework;

namespace HomeInventory.Tests.Presentation.Web;

public sealed class SubjectOptions : IOptions
{
    public static SectionPath Section => nameof(Section);
}
