using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Text.RegularExpressions; // Add this for Regex if not already present

public class GradlePreProcessor : IPostprocessBuildWithReport // or IPostGenerateGradleAndroidProject
{
    // For IPostprocessBuildWithReport, you'd implement the OnPostprocessBuild method
    public int callbackOrder { get { return 0; } }

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.Android)
        {
            // This is one way to trigger the fix, assuming you get the path correctly
            // It's more common to use IPostGenerateGradleAndroidProject
        }
    }

    // This is the method from your logs, suggesting you are using IPostGenerateGradleAndroidProject
    public void OnPostGenerateGradleAndroidProject(string path)
    {
        Debug.Log($"[GradlePreProcessor] 🔄 OnPostGenerateGradleAndroidProject: {path}");

        // Define the paths to the build.gradle files
        string launcherBuildGradlePath = Path.Combine(path, "..", "launcher", "build.gradle");
        string unityLibraryBuildGradlePath = Path.Combine(path, "build.gradle");
        string rootBuildGradlePath = Path.Combine(path, "..", "build.gradle"); // This is the main build.gradle

        FixGradleFile(launcherBuildGradlePath);
        FixGradleFile(unityLibraryBuildGradlePath);
        FixGradleFile(rootBuildGradlePath); // Ensure the root build.gradle is also processed
    }


    private void FixGradleFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"[GradlePreProcessor] Error: Gradle file not found at {filePath}");
            return;
        }

        string fileContent = File.ReadAllText(filePath);
        bool changed = false;

        // Get the actual API levels from Unity Player Settings
        int compileSdkVersion = PlayerSettings.Android.targetSdkVersion == AndroidSdkVersions.AndroidApiLevelAuto
                                ? 34 // Or a reasonable default like 33 or 34 for recent Unity versions
                                : (int)PlayerSettings.Android.targetSdkVersion;

        int targetSdkVersion = PlayerSettings.Android.targetSdkVersion == AndroidSdkVersions.AndroidApiLevelAuto
                               ? 34 // Or a reasonable default
                               : (int)PlayerSettings.Android.targetSdkVersion;

        // Use MinSdkVersion if you intend to replace COMPILE_SDK with min rather than target
        int minSdkVersion = (int)PlayerSettings.Android.minSdkVersion;


        // --- Fix compileSdk and targetSdk placeholders ---
        // Look for the literal placeholder strings and replace them
        string newContent = fileContent.Replace("compileSdk **COMPILE_SDK**", $"compileSdk {compileSdkVersion}");
        if (newContent != fileContent) { fileContent = newContent; changed = true; Debug.LogWarning($"[GradlePreProcessor] ⚠ compileSdk was missing → added compileSdk {compileSdkVersion} in {filePath}"); }

        newContent = fileContent.Replace("targetSdk **TARGET_SDK**", $"targetSdk {targetSdkVersion}");
        if (newContent != fileContent) { fileContent = newContent; changed = true; Debug.LogWarning($"[GradlePreProcessor] ⚠ targetSdk was missing → added targetSdk {targetSdkVersion} in {filePath}"); }

        // Additionally, ensure that the minSdkVersion is properly set.
        // This might not be using placeholders, but good to check.
        // Regex example for more robust replacement if the format varies
        newContent = Regex.Replace(fileContent, @"minSdkVersion\s+\d+", $"minSdkVersion {minSdkVersion}");
        if (newContent != fileContent) { fileContent = newContent; changed = true; }
        else
        {
            newContent = Regex.Replace(fileContent, @"minSdkVersion\s+[\w\d_]+", $"minSdkVersion {minSdkVersion}"); // For placeholder like 'minSdkVersion UNITY_MIN_SDK_VERSION'
            if (newContent != fileContent) { fileContent = newContent; changed = true; }
        }


        // --- Fix jcenter() deprecation ---
        // This is safer with Regex to target only within repositories blocks, but a simple replace might work too.
        if (fileContent.Contains("jcenter()"))
        {
            // This is a simple replace. For more robust replacement, consider a Regex that targets
            // repositories blocks, but for a quick fix, this often works.
            newContent = fileContent.Replace("jcenter()", "mavenCentral()");
            if (newContent != fileContent) { fileContent = newContent; changed = true; Debug.LogWarning($"[GradlePreProcessor] ⚠ jcenter() deprecated → replaced with mavenCentral() in {filePath}"); }
        }


        // --- Handle debugSymbolLevel (optional but good to fix warnings) ---
        if (!fileContent.Contains("debugSymbolLevel"))
        {
            // Find where to insert debugSymbolLevel, usually within defaultConfig or buildTypes.
            // This is a basic example, you might need to adjust the insertion point.
            string searchPattern = @"defaultConfig\s*\{";
            string replacement = $"defaultConfig {{\n        debugSymbolLevel 'full'"; // Or 'none', 'embedded'

            // Using Regex.Replace for more control over insertion
            newContent = Regex.Replace(fileContent, searchPattern, replacement, RegexOptions.Multiline);
            if (newContent != fileContent) { fileContent = newContent; changed = true; Debug.LogWarning($"[GradlePreProcessor] ⚠ debugSymbolLevel missing → added to {filePath}"); }
        }


        if (changed)
        {
            File.WriteAllText(filePath, fileContent);
            Debug.Log($"[GradlePreProcessor] ✅ Updated: {filePath}");
        }
        else
        {
            Debug.Log($"[GradlePreProcessor] ✔ No changes needed: {filePath}");
        }
    }
}