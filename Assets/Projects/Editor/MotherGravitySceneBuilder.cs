using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MotherGravitySceneBuilder : EditorWindow
{
    private StageBuildConfig config;

    [MenuItem("Tools/MotherGravity/Build Stage Scene")]
    public static void ShowWindow()
    {
        GetWindow<MotherGravitySceneBuilder>("Stage Scene Builder");
    }

    private void OnGUI()
    {
        GUILayout.Label("MotherGravity Stage Scene Builder", EditorStyles.boldLabel);
        config = (StageBuildConfig)EditorGUILayout.ObjectField("Build Config", config, typeof(StageBuildConfig), false);

        if (config == null) return;

        SerializedObject so = new SerializedObject(config);
        so.Update();
        EditorGUILayout.PropertyField(so.FindProperty("sceneOutputFolder"));
        EditorGUILayout.PropertyField(so.FindProperty("stageDataAsset"));

        GUILayout.Space(10);
        GUILayout.Label("ギミック配置", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(so.FindProperty("randomizeGimmicks"));
        if (config.randomizeGimmicks)
        {
            EditorGUILayout.PropertyField(so.FindProperty("gimmickSpawnMin"));
            EditorGUILayout.PropertyField(so.FindProperty("gimmickSpawnMax"));
        }

        GUILayout.Space(10);
        GUILayout.Label("ターゲット配置", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(so.FindProperty("randomizeTargets"));
        EditorGUILayout.PropertyField(so.FindProperty("targetPrefab"));
        EditorGUILayout.PropertyField(so.FindProperty("targetCount"));
        if (config.randomizeTargets)
        {
            EditorGUILayout.PropertyField(so.FindProperty("targetSpawnMin"));
            EditorGUILayout.PropertyField(so.FindProperty("targetSpawnMax"));
        }

        GUILayout.Space(10);
        GUILayout.Label("カスタムプレファブ配置", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(so.FindProperty("placeCustomPrefabs"));
        if (config.placeCustomPrefabs)
        {
            EditorGUILayout.PropertyField(so.FindProperty("customPrefabs"), true);
            EditorGUILayout.PropertyField(so.FindProperty("customPrefabSpawnMin"));
            EditorGUILayout.PropertyField(so.FindProperty("customPrefabSpawnMax"));
        }

        so.ApplyModifiedProperties();

        GUILayout.Space(10);
        if (GUILayout.Button("Build Scene"))
        {
            if (config.stageDataAsset == null)
            {
                EditorUtility.DisplayDialog("Error", "StageData アセットをアサインしてください。", "OK");
                return;
            }
            BuildScene(config);
        }
    }

    private void BuildScene(StageBuildConfig cfg)
    {
        string sceneName = "Stage_" + cfg.stageDataAsset.stageID;
        string scenePath = System.IO.Path.Combine(cfg.sceneOutputFolder, sceneName + ".unity");

        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject gimmickContainer = new GameObject("GimmickContainer");
        if (cfg.stageDataAsset.gimmicks != null)
        {
            foreach (var gimmick in cfg.stageDataAsset.gimmicks)
            {
                if (gimmick.gimmickPrefab != null)
                {
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(gimmick.gimmickPrefab);
                    instance.transform.SetParent(gimmickContainer.transform);
                    Vector3 basePosition = gimmick.position;
                    if (cfg.randomizeGimmicks)
                    {
                        float randX = Random.Range(cfg.gimmickSpawnMin.x, cfg.gimmickSpawnMax.x);
                        float randY = Random.Range(cfg.gimmickSpawnMin.y, cfg.gimmickSpawnMax.y);
                        basePosition += new Vector3(randX, randY, 0);
                    }
                    instance.transform.position = basePosition;
                    instance.transform.rotation = gimmick.rotation;
                }
            }
        }

        GameObject targetContainer = new GameObject("TargetContainer");
        if (cfg.targetPrefab != null && cfg.targetCount > 0)
        {
            for (int i = 0; i < cfg.targetCount; i++)
            {
                Vector3 spawnPos = cfg.randomizeTargets
                    ? new Vector3(Random.Range(cfg.targetSpawnMin.x, cfg.targetSpawnMax.x), Random.Range(cfg.targetSpawnMin.y, cfg.targetSpawnMax.y), 0)
                    : new Vector3(i * 2.0f, 0, 0);

                GameObject targetInstance = (GameObject)PrefabUtility.InstantiatePrefab(cfg.targetPrefab);
                targetInstance.transform.SetParent(targetContainer.transform);
                targetInstance.transform.position = spawnPos;
            }
        }

        if (cfg.placeCustomPrefabs && cfg.customPrefabs != null)
        {
            GameObject customContainer = new GameObject("CustomPrefabs");
            foreach (var prefab in cfg.customPrefabs)
            {
                if (prefab != null)
                {
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    instance.transform.SetParent(customContainer.transform);
                }
            }
        }
        
        var lifetimeScope = FindFirstObjectByType<GameLifetimeScope>();

        SerializedObject so = new SerializedObject(lifetimeScope);
        SerializedProperty prop = so.FindProperty("stageDataAsset");
        if (prop != null)
        {
            prop.objectReferenceValue = cfg.stageDataAsset;
            so.ApplyModifiedProperties();
        }

        bool saveOk = EditorSceneManager.SaveScene(newScene, scenePath);
        if (saveOk)
        {
            Debug.Log($"シーンを生成・保存しました: {scenePath}");
        }
        else
        {
            Debug.LogError("シーンの保存に失敗しました。");
        }
    }
}
