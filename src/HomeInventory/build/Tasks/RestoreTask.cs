using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Restore")]
public sealed class RestoreTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetRestore(
            context.Solution,
            new DotNetRestoreSettings
            {
                Verbosity = context.Verbosity,
            });
    }
}
