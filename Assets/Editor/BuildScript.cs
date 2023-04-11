// -------------------------------------------------------------------------------------------------
// Assets/Editor/JenkinsBuild.cs
// -------------------------------------------------------------------------------------------------
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Reporting;
  
// ------------------------------------------------------------------------
// https://docs.unity3d.com/Manual/CommandLineArguments.html
// ------------------------------------------------------------------------
public class JenkinsBuild {
    private static readonly string[] EnabledScenes = FindEnabledEditorScenes();
  
    // ------------------------------------------------------------------------
    // called from Jenkins
    // ------------------------------------------------------------------------
    // public static void BuildMacOS()
    // {
    //     var args = FindArgs();
    //
    //     string fullPathAndName = args.targetDir + args.appName + ".app";
    //     BuildProject(EnabledScenes, fullPathAndName, BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX, BuildOptions.None);
    // }
    //
    // ------------------------------------------------------------------------
    // called from Jenkins
    // ------------------------------------------------------------------------
    public static void BuildWindows64()
    {
        var args = FindArgs();
 
        var fullPathAndName = args.targetDir + args.appName;
        BuildProject(EnabledScenes, fullPathAndName, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, BuildOptions.None);
    }
    
    // ------------------------------------------------------------------------
    // called from Jenkins
    // ------------------------------------------------------------------------
    public static void BuildAndroid()
    {
        var args = FindArgs();
 
        var fullPathAndName = args.targetDir + args.appName + ".apk";
        BuildProject(EnabledScenes, fullPathAndName, BuildTargetGroup.Android, BuildTarget.Android, BuildOptions.None);
    }
    
 
    // ------------------------------------------------------------------------
    // called from Jenkins
    // ------------------------------------------------------------------------
    // public static void BuildLinux()
    // {
    //     var args = FindArgs();
    //
    //     string fullPathAndName = args.targetDir + args.appName;
    //     BuildProject(EnabledScenes, fullPathAndName, BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64, BuildOptions.None);
    // }
 
    private static Args FindArgs()
    {
        var returnValue = new Args();
 
        // find: -executeMethod
        //   +1: JenkinsBuild.BuildMacOS
        //   +2: FindTheGnome
        //   +3: D:\Jenkins\Builds\Find the Gnome\47\output
        var args = System.Environment.GetCommandLineArgs();
        var execMethodArgPos = -1;
        var allArgsFound = false;
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "-executeMethod")
            {
                execMethodArgPos = i;
            }
            var realPos = execMethodArgPos == -1 ? -1 : i - execMethodArgPos - 2;
            switch (realPos)
            {
                case < 0:
                    continue;
                case 0:
                    returnValue.appName = args[i];
                    break;
                case 1:
                {
                    returnValue.targetDir = args[i];
                    if (!returnValue.targetDir.EndsWith(System.IO.Path.DirectorySeparatorChar + ""))
                        returnValue.targetDir += System.IO.Path.DirectorySeparatorChar;

                    returnValue.targetDir = returnValue.targetDir.Replace("\"", "");
 
                    allArgsFound = true;
                    break;
                }
            }
        }
 
        if (!allArgsFound)
            System.Console.WriteLine("[JenkinsBuild] Incorrect Parameters for -executeMethod Format: -executeMethod JenkinsBuild.BuildWindows64 <app name> <output dir>");
        
        System.Console.WriteLine($"[JenkinsBuild] Parameters for -executeMethod <app name>: {returnValue.appName} <output dir>: {returnValue.targetDir}");
 
        return returnValue;
    }
 
 
    // ------------------------------------------------------------------------
    // ------------------------------------------------------------------------
    private static string[] FindEnabledEditorScenes(){
        return (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
    }
  
    // ------------------------------------------------------------------------
    // e.g. BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX
    // ------------------------------------------------------------------------
    private static void BuildProject(string[] scenes, string targetDir, BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, BuildOptions buildOptions)
    {
        System.Console.WriteLine("[JenkinsBuild] Building: " + targetDir + " buildTargetGroup:" + buildTargetGroup.ToString() + " buildTarget:" + buildTarget.ToString());
  
        // https://docs.unity3d.com/ScriptReference/EditorUserBuildSettings.SwitchActiveBuildTarget.html
        var switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        if (switchResult)
        {
            System.Console.WriteLine("[JenkinsBuild] Successfully changed Build Target to: " + buildTarget.ToString());
        }
        else
        {
            System.Console.WriteLine("[JenkinsBuild] Unable to change Build Target to: " + buildTarget.ToString() + " Exiting...");
            return;
        }
  
        // https://docs.unity3d.com/ScriptReference/BuildPipeline.BuildPlayer.html
        BuildReport buildReport = BuildPipeline.BuildPlayer(scenes, targetDir, buildTarget, buildOptions);
        BuildSummary buildSummary = buildReport.summary;
        if (buildSummary.result == BuildResult.Succeeded)
        {
            System.Console.WriteLine("[JenkinsBuild] Build Success: Time:" + buildSummary.totalTime + " Size:" + buildSummary.totalSize + " bytes");
        }
        else
        {
            System.Console.WriteLine("[JenkinsBuild] Build Failed: Time:" + buildSummary.totalTime + " Total Errors:" + buildSummary.totalErrors);
        }
    }
 
    private class Args
    {
        public string appName = "AppName";
        public string targetDir = "~/Desktop";
    }
}

