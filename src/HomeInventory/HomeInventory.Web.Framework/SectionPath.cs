using Microsoft.Extensions.Configuration;

namespace HomeInventory.Web.Framework;

public sealed record SectionPath(string Path)
{
    public Option<SectionPath> GetParentOptional() => ConfigurationPath.GetParentPath(Path).NoneIfNull().Map(ToSectionPath);

    public static implicit operator SectionPath(string path) => ToSectionPath(path);

    public static implicit operator string(SectionPath path) => path.ToString();

    public static SectionPath operator /(SectionPath left, SectionPath right) => Divide(left, right);

    public static SectionPath ToSectionPath(string path) => new(path);

    public static SectionPath Divide(SectionPath left, SectionPath right) => ConfigurationPath.Combine(left, right);

    public override string ToString() => Path;
}
