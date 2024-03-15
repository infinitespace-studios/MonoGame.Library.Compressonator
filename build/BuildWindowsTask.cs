using Cake.Common.Tools.MSBuild;

namespace BuildScripts;

[TaskName("Build Windows")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildLibraryTask))]
public sealed class BuildWindowsTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnWindows();

    public override void Run(BuildContext context)
    {
        var buildWorkingDir = "compressonator";
        context.StartProcess("python3", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "build/fetch_dependencies.py"});
        context.StartProcess ("build/windows_build_sdk.bat", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "" });
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "-DOPTION_ENABLE_ALL_APPS=OFF -DOPTION_BUILD_APPS_CMP_CLI=ON -DOPTION_BUILD_ASTC=ON -DCMAKE_OSX_ARCHITECTURES=\"x86_64;arm64\" CMakeLists.txt" });
        context.StartProcess("make", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "" });
        var files = Directory.GetFiles(System.IO.Path.Combine(buildWorkingDir, "bin"), "libcompressonator.*.*.*.dll", SearchOption.TopDirectoryOnly);
        context.CopyFile(files[0], $"{context.ArtifactsDir}/libcompressonator.dll");
    }
}
