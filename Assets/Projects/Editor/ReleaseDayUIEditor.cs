namespace AIBirthGame
{
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class ReleaseDayUIEditor
{
    // 「GameObject > UI > ReleaseDay UI」から呼び出すメニュー項目
    [MenuItem("GameObject/UI/ReleaseDay UI", false, 10)]
    public static void CreateReleaseDayUI(MenuCommand menuCommand)
    {
        // シーン内にCanvasがあるかチェック
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            // なければ新規作成
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // CanvasScaler と GraphicRaycaster を追加
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            // ヒエラルキーに登録
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");
        }

        // UIパネル作成
        GameObject panelGO = new GameObject("ReleaseDayPanel");
        // Canvas 以下なら RectTransform が自動的に付与されるので設定は不要
        panelGO.transform.SetParent(canvas.transform, false);

        // 背景用Imageを追加（任意の色で半透明）
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);

        // 表示/非表示制御用にCanvasGroupを追加
        panelGO.AddComponent<CanvasGroup>();

        // パネルのサイズを設定
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(400, 200);
        panelRect.anchoredPosition = Vector2.zero;

        // テキストオブジェクトを作成
        GameObject textGO = new GameObject("ReleaseDayText");
        textGO.transform.SetParent(panelGO.transform, false);
        // RectTransform は Canvas の子なら自動でアタッチ
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // TextMeshProUGUIを追加してテキストを設定
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = "リリースデー";
        tmp.fontSize = 36;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        // Undo 操作の登録
        Undo.RegisterCreatedObjectUndo(panelGO, "Create ReleaseDay UI");

        // 作成したパネルを選択状態にする
        Selection.activeGameObject = panelGO;
    }
}

}