using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class DuplicateClassFinder : EditorWindow
{
    [MenuItem("Tools/Find Duplicate Classes")]
    public static void FindDuplicates()
    {
        string[] scriptFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        Dictionary<string, List<string>> classLocations = new Dictionary<string, List<string>>();

        foreach (string file in scriptFiles)
        {
            string[] lines = File.ReadAllLines(file);

            foreach (string line in lines)
            {
                if (line.TrimStart().StartsWith("public class") || line.TrimStart().StartsWith("class"))
                {
                    string[] parts = line.Trim().Split(' ');
                    int index = System.Array.IndexOf(parts, "class");
                    if (index != -1 && index < parts.Length - 1)
                    {
                        string className = parts[index + 1];

                        // لو فيه { بعد اسم الكلاس نشيلها
                        className = className.Replace("{", "").Trim();

                        if (!classLocations.ContainsKey(className))
                        {
                            classLocations[className] = new List<string>();
                        }
                        classLocations[className].Add(file);
                    }
                }
            }
        }

        foreach (var kvp in classLocations)
        {
            if (kvp.Value.Count > 1)
            {
                Debug.LogError($"⚠️ Duplicate class found: {kvp.Key}");
                foreach (string path in kvp.Value)
                {
                    Debug.Log(" - " + path);
                }
            }
        }

        Debug.Log("✅ Duplicate class scan completed!");
    }
}
