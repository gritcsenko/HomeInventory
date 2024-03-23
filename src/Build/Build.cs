using System;
using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Info, x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    Target Info => _ => _
        .Before(Restore)
        .Executes(() => BuildTools.DotNetInfo(s => s.SetDisplayVersion(true).SetDisplayRuntimes(true).SetDisplaySdks(true)));

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() => DotNetClean(s => s.SetProject(Solution).SetConfiguration(Configuration)));

    Target Restore => _ => _
        .Executes(() => DotNetRestore(s => s.SetProjectFile(Solution)));

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() => DotNetBuild(s => s.SetProjectFile(Solution).SetConfiguration(Configuration).SetNoRestore(true)));

}

public static class BuildTools
{
    public static IReadOnlyCollection<Output> DotNetInfo(Configure<DotNetInfoSettings> configurator)
    {
        return DotNetInfo(configurator(new DotNetInfoSettings()));
    }

    public static IReadOnlyCollection<Output> DotNetInfo(DotNetInfoSettings? toolSettings = null)
    {
        toolSettings ??= new DotNetInfoSettings();
        using var process = ProcessTasks.StartProcess(toolSettings);
        toolSettings.ProcessExitHandler.Invoke(toolSettings, process.AssertWaitForExit());
        return process.Output;
    }
}

[Serializable]
public class DotNetInfoSettings : ToolSettings
{
    public override string ProcessToolPath => base.ProcessToolPath ?? DotNetPath;
    public override Action<OutputType, string> ProcessLogger => base.ProcessLogger ?? DotNetLogger;
    public override Action<ToolSettings, IProcess> ProcessExitHandler => base.ProcessExitHandler ?? DotNetExitHandler;

    public virtual bool? DisplayRuntimes { get; set; }

    public virtual bool? DisplaySdks { get; set; }

    public virtual bool? DisplayVersion { get; set; }

    protected override Arguments ConfigureProcessArguments(Arguments arguments)
    {
        arguments
            .Add("--info")
            .Add("--list-runtimes", DisplayRuntimes)
            .Add("--list-sdks", DisplaySdks)
            .Add("--version", DisplayVersion);
        return base.ConfigureProcessArguments(arguments);
    }
}

public static class DotNetInfoSettingsExtensions
{
    public static T SetDisplayRuntimes<T>(this T toolSettings, bool? displayRuntimes) where T : DotNetInfoSettings
    {
        toolSettings = toolSettings.NewInstance();
        toolSettings.DisplayRuntimes = displayRuntimes;
        return toolSettings;
    }

    public static T ResetDisplayRuntimes<T>(this T toolSettings) where T : DotNetInfoSettings
    {
        toolSettings = toolSettings.NewInstance();
        toolSettings.DisplayRuntimes = null;
        return toolSettings;
    }

    public static T SetDisplaySdks<T>(this T toolSettings, bool? displaySdks) where T : DotNetInfoSettings
    {
        toolSettings = toolSettings.NewInstance();
        toolSettings.DisplaySdks = displaySdks;
        return toolSettings;
    }

    public static T ResetDisplaySdks<T>(this T toolSettings) where T : DotNetInfoSettings
    {
        toolSettings = toolSettings.NewInstance();
        toolSettings.DisplaySdks = null;
        return toolSettings;
    }

    public static T SetDisplayVersion<T>(this T toolSettings, bool? displayVersion) where T : DotNetInfoSettings
    {
        toolSettings = toolSettings.NewInstance();
        toolSettings.DisplayVersion = displayVersion;
        return toolSettings;
    }

    public static T ResetDisplayVersion<T>(this T toolSettings) where T : DotNetInfoSettings
    {
        toolSettings = toolSettings.NewInstance();
        toolSettings.DisplayVersion = null;
        return toolSettings;
    }
}
