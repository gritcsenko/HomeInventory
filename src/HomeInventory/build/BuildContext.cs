using Cake.Common;
using Cake.Common.Tools.DotNet;
using Cake.Core;
using Cake.Frosting;

namespace Build;

public class BuildContext(ICakeContext context) : FrostingContext(context)
{
    public string Solution { get; } = "../HomeInventory.sln";

    public string Tests { get; } = "../HomeInventory.Tests";

    public string BuildConfiguration { get; } = context.Argument("configuration", "Release");

    public DotNetVerbosity Verbosity { get; } = DotNetVerbosity.Normal;
}
