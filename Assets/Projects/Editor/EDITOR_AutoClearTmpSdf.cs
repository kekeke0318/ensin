using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;

[InitializeOnLoad]
public static class AutoClearTmpSdf
{
    static AutoClearTmpSdf()
    {
        // プレイモードの状態が変わるたびにコールバック
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            ClearAllTmpFontAssets();
        }
    }

    [MenuItem("Tools/Clear SDF Cache")]
    private static void ClearAllTmpFontAssets()
    {
        // プロジェクト内のすべての TMP_FontAsset を検索
        string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);

            // Dynamicだけ処理する
            if (fontAsset.atlasPopulationMode != AtlasPopulationMode.Dynamic) continue;

            fontAsset.ClearFontAssetData(true);

            EditorUtility.SetDirty(fontAsset);
        }

        // いるかいらないか不明
        //TMP_ResourceManager.ClearFontAssetGlyphCache();

        AssetDatabase.SaveAssets();
        Debug.Log($"Cleared glyph tables in {guids.Length} TMP_FontAsset(s).");
    }
}