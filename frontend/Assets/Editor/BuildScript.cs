// Assets/Editor/BuildScript.cs
// Script apelat de CI prin -executeMethod BuildScript.PerformBuild
// Pune-l în Assets/Editor/ în proiectul tău Unity.

using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.Linq;

public class BuildScript
{
    public static void PerformBuild()
    {
        // Citește parametrul -buildPath din argumentele CI
        string buildPath = GetArgument("-buildPath");
        if (string.IsNullOrEmpty(buildPath))
        {
            buildPath = "Builds/Windows/PuzzleGame.exe";
        }

        Debug.Log($"[CI] Starting build → {buildPath}");

        // Ia toate scenele active din Build Settings
        string[] scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            Debug.LogError("[CI] No scenes found in Build Settings!");
            EditorApplication.Exit(1);
            return;
        }

        Debug.Log($"[CI] Building {scenes.Length} scene(s): {string.Join(", ", scenes)}");

        var buildOptions = new BuildPlayerOptions
        {
            scenes           = scenes,
            locationPathName = buildPath,
            target           = BuildTarget.StandaloneWindows64,
            options          = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[CI] Build succeeded! Size: {summary.totalSize / 1024 / 1024} MB");
            EditorApplication.Exit(0);
        }
        else
        {
            Debug.LogError($"[CI] Build FAILED with {summary.totalErrors} error(s).");
            EditorApplication.Exit(1);
        }
    }

    private static string GetArgument(string name)
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == name)
                return args[i + 1];
        }
        return null;
    }
}
