using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EDITOR_ScriptableObjectCreator : ScriptableWizard
{

    [SerializeField]
    private MonoScript _targetScript;
    [SerializeField]
    private string _outputObjectName;
    [SerializeField]
    private string _outputDirectory;

    private static readonly string _outputPrefix = "";
    private static readonly string _outputSuffix = ".asset";

    private MonoScript _targetCache = null;

    [MenuItem("ScriptableObject/Creator")]
    private static void Open()
    {
        EDITOR_ScriptableObjectCreator window = DisplayWizard<EDITOR_ScriptableObjectCreator>("Scriptable Object Creator");

        if (Selection.activeObject is MonoScript)
        {
            window.SetTargetScript(Selection.activeObject as MonoScript);
            window.OnWizardUpdate();
        }
    }

    public void SetTargetScript(MonoScript target)
    {
        _targetScript = target;
    }

    private void OnWizardCreate()
    {
        SafeCreateDirectory(_outputDirectory);
        var createdObject = CreateInstance(_targetScript.GetClass());
        var outputPath = Path.Combine(_outputDirectory, _outputObjectName + _outputSuffix);
        var uniqueOutputPath = AssetDatabase.GenerateUniqueAssetPath(outputPath);
        AssetDatabase.CreateAsset(createdObject, uniqueOutputPath);
        AssetDatabase.Refresh();

        EditorGUIUtility.PingObject(createdObject);
    }

    public void OnWizardUpdate()
    {
        if (_targetScript != null && _targetCache != _targetScript)
        {
            _outputObjectName = _outputPrefix + _targetScript.name;
            //_outputDirectory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_targetScript)).Replace("Assets\\Scripts\\", "Assets\\AssetBundles\\");
            _outputDirectory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_targetScript));
            _targetCache = _targetScript;
        }
        isValid = _targetScript != null && !_targetScript.GetType().IsAbstract && !string.IsNullOrEmpty(_outputObjectName) && !string.IsNullOrEmpty(_outputDirectory);
    }

    private void SafeCreateDirectory(string path)
    {
        var currentPath = "";
        var splitChar = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        foreach (var dir in path.Split(splitChar))
        {
            var parent = currentPath;
            currentPath = Path.Combine(currentPath, dir);
            if (!AssetDatabase.IsValidFolder(currentPath))
            {
                AssetDatabase.CreateFolder(parent, dir);
            }
        }
    }
}