using Cake.Common.Tools.DotNet;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Clean")]
public sealed class CleanTask : FrostingTask<Context>
{
    public override void Run(Context context)
    {
        context.DotNetClean(context.Solution);
    }
}
