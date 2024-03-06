using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Frosting;

namespace Build;

public sealed class Lifetime : FrostingLifetime<Context>
{
    public override void Setup(Context context, ISetupContext info)
    {
        context.IsRunningInCI = context.HasEnvironmentVariable("TF_BUILD");
        context.Information("Is Running in CI : {0}", context.IsRunningInCI);
        if (context.Settings.Version == "auto" && !context.IsRunningInCI)
        {
            // Temporarily commit all changes to prevent checking in scripted changes like versioning.
            context.Git("add", ".");
            context.Git("commit", "--allow-empty", "-m", "'backup'");
        }
    }

    public override void Teardown(Context context, ITeardownContext info)
    {
        if (context.Settings.Version == "auto" && !context.IsRunningInCI)
        {
            // Undoes the script changes to all tracked files.
            context.Git("reset", "--hard");

            // Undoes the setup commit keeping file states as before this build script ran.
            context.Git("reset", "HEAD^");
        }
    }
}
