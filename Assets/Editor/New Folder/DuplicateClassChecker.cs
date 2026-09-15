using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class DuplicateClassChecker : EditorWindow
{
    private Vector2 scrollPos;
    private Dictionary<string, List<string>> duplicateClasses;

    [MenuItem("Tools/Check Duplicate Classes (GUI)")]
    public static void ShowWindow()
    {
        DuplicateClassChecker window = GetWindow<DuplicateClassChecker>("Duplicate Classes");
        window.CheckDuplicateClasses();
        window.Show();
    }

    private void OnGUI()
    {
        if (duplicateClasses == null)
        {
            GUILayout.Label("⚠️ Press the button to check for duplicate classes.", EditorStyles.boldLabel);
            if (GUILayout.Button("Check Now"))
            {
                CheckDuplicateClasses();
            }
            return;
        }

        GUILayout.Label("🔎 Duplicate Class Report", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        if (duplicateClasses.Count == 0)
        {
            GUILayout.Label("✅ No duplicate classes found.", EditorStyles.helpBox);
        }
        else
        {
            foreach (var kvp in duplicateClasses)
            {
                GUILayout.Label($"⚠️ Class: {kvp.Key}", EditorStyles.boldLabel);

                foreach (string path in kvp.Value)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(" - " + path, EditorStyles.wordWrappedLabel);

                    if (GUILayout.Button("Open", GUILayout.Width(60)))
                    {
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, 1);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(5);
            }
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Recheck"))
        {
            CheckDuplicateClasses();
        }
    }

    private void CheckDuplicateClasses()
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
                        if (!classLocations.ContainsKey(className))
                        {
                            classLocations[className] = new List<string>();
                        }
                        classLocations[className].Add(file);
                    }
                }
            }
        }

        duplicateClasses = new Dictionary<string, List<string>>();

        foreach (var kvp in classLocations)
        {
            if (kvp.Value.Count > 1)
            {
                duplicateClasses[kvp.Key] = kvp.Value;
            }
        }
    }
}
