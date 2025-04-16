using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class MotherGravitySceneBuilder : EditorWindow
{
    // StageData のアセットを指定しておけば、GameLifetimeScope に渡せる
    private StageData stageDataAsset;

    [MenuItem("Tools/MotherGravity/Build Scene")]
    public static void ShowWindow()
    {
        GetWindow<MotherGravitySceneBuilder>("Build MotherGravity Scene");
    }

    private void OnGUI()
    {
        GUILayout.Label("MotherGravity Scene Builder", EditorStyles.boldLabel);
        stageDataAsset = (StageData)EditorGUILayout.ObjectField("Stage Data Asset", stageDataAsset, typeof(StageData), false);

        if (GUILayout.Button("Build Scene"))
        {
            BuildScene();
        }
    }

    private void BuildScene()
    {
        // 新しいシーンを作成
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // GameLifetimeScope を持つ空の GameObject を作成
        GameObject lifetimeScopeGO = new GameObject("GameLifetimeScope");
        // GameLifetimeScope は MonoBehaviour を継承した DI 登録用コンポーネント
        // ※事前に作成済みの GameLifetimeScope スクリプトがプロジェクト内に存在している前提
        var lifetimeScope = lifetimeScopeGO.AddComponent<GameLifetimeScope>();

        // ※エディタ上で StageData アセットが設定されている場合、GameLifetimeScope 内で利用するようシリアライズフィールドにセットする処理を追加
        // ここではシンプルに、lifetimeScope のスクリプト側で [SerializeField] として StageData を受け取れるようにしておく想定です
        if (stageDataAsset != null)
        {
            SerializedObject so = new SerializedObject(lifetimeScope);
            so.FindProperty("stageDataAsset").objectReferenceValue = stageDataAsset;
            so.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogWarning("StageData Asset が未設定です。後から手動でアサインしてください。");
        }

        // InputController を保持する GameObject を作成（必要な場合はここに追加）
        GameObject inputControllerGO = new GameObject("InputController");
        // InputController は MonoBehaviour で、ドラッグ操作などを行います
        inputControllerGO.AddComponent<InputController>();

        // MainCamera が存在しなければ作成する
        if (Camera.main == null)
        {
            GameObject cameraGO = new GameObject("Main Camera");
            Camera camera = cameraGO.AddComponent<Camera>();
            camera.tag = "MainCamera";
            // 必要に応じてカメラの位置や設定を調整
            cameraGO.transform.position = new Vector3(0, 0, -10);
        }

        // シーン保存
        string scenePath = "Assets/MotherGravityScene.unity";
        bool saveOk = EditorSceneManager.SaveScene(newScene, scenePath);
        if (saveOk)
        {
            Debug.Log($"シーンが構築されました。保存先: {scenePath}");
        }
        else
        {
            Debug.LogError("シーンの保存に失敗しました。");
        }
    }
}
