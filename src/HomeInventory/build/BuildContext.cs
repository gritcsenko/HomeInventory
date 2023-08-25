using Cake.Core;
using Cake.Frosting;

namespace Build;

public class BuildContext : FrostingContext
{
    public string Solution { get; } = "./HomeInventory.sln";
    public string Release { get; } = "Release";

    public bool Delay { get; set; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        Delay = context.Arguments.HasArgument("delay");
    }
}
