using Cake.Common.Tools.DotNet;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Restore")]
public sealed class RestoreTask : FrostingTask<Context>
{
    public override void Run(Context context) => context.DotNetRestore(context.Solution, context.ToDotNetRestoreSettings());
}
