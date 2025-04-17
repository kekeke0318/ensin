using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン（UIとレベルデザイン）を自動構築するエディタスクリプト
/// 手作業でシーン構築せずこのクラスから行うことでミスを防いだり、差し替えを容易にする目的がある
/// </summary>
public class MotherGravitySceneBuilder : EditorWindow
{
    // 対象の StageData アセット
    private StageData stageDataAsset;

    // シーンの出力フォルダ設定（デフォルトは "Assets" フォルダ）
    private string sceneOutputFolder = "Assets/";

    // ギミックのランダム配置オプション
    private bool randomizeGimmicks = false;
    private Vector2 gimmickSpawnMin = new Vector2(-3f, -3f);
    private Vector2 gimmickSpawnMax = new Vector2(3f, 3f);

    // ターゲット（Star）のランダム配置オプション
    private bool randomizeTargets = false;
    private GameObject targetPrefab;
    private int targetCount = 3;
    private Vector2 targetSpawnMin = new Vector2(-5f, -5f);
    private Vector2 targetSpawnMax = new Vector2(5f, 5f);

    [MenuItem("Tools/MotherGravity/Build Stage Scene")]
    public static void ShowWindow()
    {
        GetWindow<MotherGravitySceneBuilder>("Stage Scene Builder");
    }

    private void OnGUI()
    {
        // デフォルトのインスペクタ描画風に各パラメータを設定
        GUILayout.Label("MotherGravity Stage Scene Builder", EditorStyles.boldLabel);
        
        stageDataAsset = (StageData)EditorGUILayout.ObjectField("Stage Data Asset", stageDataAsset, typeof(StageData), false);
        sceneOutputFolder = EditorGUILayout.TextField("Scene Output Folder", sceneOutputFolder);

        GUILayout.Space(10);
        GUILayout.Label("ギミック配置オプション", EditorStyles.boldLabel);
        randomizeGimmicks = EditorGUILayout.Toggle("ランダム配置する", randomizeGimmicks);
        if(randomizeGimmicks)
        {
            gimmickSpawnMin = EditorGUILayout.Vector2Field("配置範囲（最小）", gimmickSpawnMin);
            gimmickSpawnMax = EditorGUILayout.Vector2Field("配置範囲（最大）", gimmickSpawnMax);
        }

        GUILayout.Space(10);
        GUILayout.Label("ターゲット配置オプション", EditorStyles.boldLabel);
        randomizeTargets = EditorGUILayout.Toggle("ランダム配置する", randomizeTargets);
        targetPrefab = (GameObject)EditorGUILayout.ObjectField("Target Prefab", targetPrefab, typeof(GameObject), false);
        targetCount = EditorGUILayout.IntField("Target Count", targetCount);
        if(randomizeTargets)
        {
            targetSpawnMin = EditorGUILayout.Vector2Field("配置範囲（最小）", targetSpawnMin);
            targetSpawnMax = EditorGUILayout.Vector2Field("配置範囲（最大）", targetSpawnMax);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Build Scene"))
        {
            if (stageDataAsset == null)
            {
                EditorUtility.DisplayDialog("Error", "StageData アセットをアサインしてください。", "OK");
                return;
            }
            BuildScene();
        }
    }

    private void BuildScene()
    {
        // シーン名は "Stage_[stageID].unity"
        string sceneName = "Stage_" + stageDataAsset.stageID;
        string scenePath = System.IO.Path.Combine(sceneOutputFolder, sceneName + ".unity");

        // 新規シーン作成（EmptyScene）
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Main Camera の作成
        if (Camera.main == null)
        {
            GameObject cameraGO = new GameObject("Main Camera");
            Camera camera = cameraGO.AddComponent<Camera>();
            camera.tag = "MainCamera";
            cameraGO.transform.position = new Vector3(0, 0, -10);
        }

        // GameLifetimeScope を持つ GameObject の作成
        GameObject lifetimeScopeGO = new GameObject("GameLifetimeScope");
        // GameLifetimeScope は DI 用の MonoBehaviour コンポーネント（事前にプロジェクト内に実装済みのもの）
        var lifetimeScope = lifetimeScopeGO.AddComponent<GameLifetimeScope>();
        
        // GameLifetimeScope に stageDataAsset をセットするため、シリアライズ更新
        SerializedObject so = new SerializedObject(lifetimeScope);
        SerializedProperty prop = so.FindProperty("stageDataAsset");
        if (prop != null)
        {
            prop.objectReferenceValue = stageDataAsset;
            so.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogWarning("GameLifetimeScope に stageDataAsset フィールドが見つかりません。手動でアサインしてください。");
        }

        // ギミック配置用のコンテナ作成
        GameObject gimmickContainer = new GameObject("GimmickContainer");

        // StageData の gimmicks 配列からギミックを配置
        if (stageDataAsset.gimmicks != null)
        {
            foreach (var gimmick in stageDataAsset.gimmicks)
            {
                if (gimmick.gimmickPrefab != null)
                {
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(gimmick.gimmickPrefab);
                    instance.transform.SetParent(gimmickContainer.transform);
                    // 基本の位置は設定された gimmick.position だが、ランダム配置がオンならオフセットを加える
                    Vector3 basePosition = gimmick.position;
                    if(randomizeGimmicks)
                    {
                        float randX = Random.Range(gimmickSpawnMin.x, gimmickSpawnMax.x);
                        float randY = Random.Range(gimmickSpawnMin.y, gimmickSpawnMax.y);
                        basePosition += new Vector3(randX, randY, 0);
                    }
                    instance.transform.position = basePosition;
                    instance.transform.rotation = gimmick.rotation;
                }
            }
        }

        // ターゲット配置用のコンテナ作成
        GameObject targetContainer = new GameObject("TargetContainer");
        if(targetPrefab != null && targetCount > 0)
        {
            for (int i = 0; i < targetCount; i++)
            {
                // ランダム配置処理（ランダムオフセット: 設定されていれば対象範囲内、オフなら stageData で指定した固定位置など）
                Vector3 spawnPos;
                if(randomizeTargets)
                {
                    float randX = Random.Range(targetSpawnMin.x, targetSpawnMax.x);
                    float randY = Random.Range(targetSpawnMin.y, targetSpawnMax.y);
                    spawnPos = new Vector3(randX, randY, 0);
                }
                else
                {
                    // ランダムでなく、均等配置など別のロジックにする場合はここを変更
                    spawnPos = new Vector3(i * 2.0f, 0, 0);
                }
                GameObject targetInstance = (GameObject)PrefabUtility.InstantiatePrefab(targetPrefab);
                targetInstance.transform.SetParent(targetContainer.transform);
                targetInstance.transform.position = spawnPos;
            }
        }

        // 必要に応じて、タイムリミットや目標 Star 数など他の StageData に基づくオブジェクト配置処理を追加可能

        // シーン保存（既存シーンがあれば上書き）
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
