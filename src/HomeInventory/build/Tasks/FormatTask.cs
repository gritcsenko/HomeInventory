﻿using Cake.Common.Tools.DotNet;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Format")]
[IsDependentOn(typeof(RestoreTask))]
public sealed class FormatTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context) => context.DotNetFormat(context.Solution, context.ToDotNetFormatSettings());
}
