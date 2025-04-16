using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class EDITOR_CreateScriptFromClipboard
{
    const string NAMESPACE_NAME = "AIBirthGame";

    [MenuItem("Assets/Create Script From Clipboard", false)]
    private static void CreateScriptFromClipboardMenuItem()
    {
        string clipboardText = GUIUtility.systemCopyBuffer;
        if (string.IsNullOrEmpty(clipboardText))
        {
            EditorUtility.DisplayDialog("Error", "Clipboard is empty.", "OK");
            return;
        }

        // Extract the class name from the clipboard text
        string className = ExtractClassName(clipboardText);
        if (string.IsNullOrEmpty(className))
        {
            EditorUtility.DisplayDialog("Error", "No valid class name found in clipboard.", "OK");
            return;
        }

        string path = GetSelectedPathOrFallback();
        string filePath = Path.Combine(path, className + ".cs");

        // Check if the file already exists
        if (File.Exists(filePath))
        {
            if (!EditorUtility.DisplayDialog("File exists", "File already exists. Overwrite?", "Yes", "No"))
            {
                return;
            }
        }

        // Add or replace namespace
        //clipboardText = AddOrReplaceNamespace(clipboardText, NAMESPACE_NAME);

        // Write the clipboard text to the new script file
        File.WriteAllText(filePath, clipboardText);

        // Refresh the AssetDatabase to show the new script file
        AssetDatabase.Refresh();
        //EditorUtility.DisplayDialog("Success", $"Script {className}.cs created successfully at {path}", "OK");
    }

    private static string ExtractClassName(string scriptText)
    {
        Match match = Regex.Match(scriptText, @"public.*?[class|struct]\s+(\w+)");
        return match.Success ? match.Groups[1].Value : null;
    }

    private static string GetSelectedPathOrFallback()
    {
        string path = "Assets";

        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }

        return path;
    }

    private static string AddOrReplaceNamespace(string scriptText, string namespaceName)
    {
        if (string.IsNullOrEmpty(namespaceName))
        {
            return scriptText;
        }

        string pattern = @"namespace\s+\w+";
        string replacement = $"namespace {namespaceName}";

        if (Regex.IsMatch(scriptText, pattern))
        {
            scriptText = Regex.Replace(scriptText, pattern, replacement);
        }
        else
        {
            scriptText = $"namespace {namespaceName}\n{{\n{scriptText}\n}}";
        }

        return scriptText;
    }
}
