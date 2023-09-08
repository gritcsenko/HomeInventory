using Cake.Common;
using Cake.Common.Tools.DotNet;
using Cake.Core;
using Cake.Frosting;

namespace Build;

public class BuildContext : FrostingContext
{
    public string Solution { get; } = "../HomeInventory.sln";

    public string Tests { get; } = "../HomeInventory.Tests";

    public string BuildConfiguration { get; }

    public DotNetVerbosity Verbosity { get; } = DotNetVerbosity.Normal;

    public BuildContext(ICakeContext context)
        : base(context)
    {
        BuildConfiguration = context.Argument("configuration", "Release");
    }
}
