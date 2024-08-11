﻿using Microsoft.Extensions.Configuration;

namespace HomeInventory.Web.Framework;

public sealed class SectionPath(string path)
{
    private readonly string _path = path;

    public Option<SectionPath> GetParentOptional()
    {
        var parent = ConfigurationPath.GetParentPath(_path);
        return parent is null ? OptionNone.Default : (SectionPath)parent;
    }

    public static implicit operator SectionPath(string path) => ToSectionPath(path);

    public static implicit operator string(SectionPath path) => path.ToString();

    public static SectionPath operator /(SectionPath left, SectionPath right) => Divide(left, right);

    public static SectionPath ToSectionPath(string path) => new(path);

    public static SectionPath Divide(SectionPath left, SectionPath right) => ConfigurationPath.Combine(left, right);

    public override string ToString() => _path;
}
