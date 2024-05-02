using Cake.Common.Tools.DotNet;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Format")]
[IsDependentOn(typeof(RestoreTask))]
public sealed class FormatTask : FrostingTask<Context>
{
    public override void Run(Context context) => context.DotNetFormat(context.Solution, context.ToDotNetFormatSettings());
}
