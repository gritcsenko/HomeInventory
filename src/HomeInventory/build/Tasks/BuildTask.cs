using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Build")]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetRestore(
            context.Solution,
            new DotNetRestoreSettings
            {
                Verbosity = DotNetVerbosity.Minimal,
            });
        context.DotNetBuild(
            context.Solution,
            new DotNetBuildSettings
            {
                Configuration = context.Release,
                NoIncremental = true,
                NoDependencies = true,
                NoRestore = true,
                NoLogo = true,
                Verbosity = DotNetVerbosity.Minimal,
                MSBuildSettings = new DotNetMSBuildSettings
                {
                    ContinuousIntegrationBuild = true,
                }
            });
    }
}
