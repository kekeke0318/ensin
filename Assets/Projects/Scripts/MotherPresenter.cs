using UnityEngine;

public class MotherPresenter
{
    private readonly StageData stageData;
    
    // コンストラクタで対応するステージデータを DI 経由などで受け取る
    public MotherPresenter(StageData stageData)
    {
        this.stageData = stageData;
    }
    
    // クリア時の会話演出を実行
    public void PresentClearDialogue()
    {
        // 例：UI にテキストを表示するなどの処理（ここでは Debug.Log で簡略化）
        Debug.Log($"ステージ {stageData.stageID} クリア: {stageData.dialogueSet.dialogueClear}");
        
        // DOTween を用いたアニメーション演出などの実装もここで行う
    }
    
    // 失敗時の会話演出を実行
    public void PresentFailDialogue()
    {
        Debug.Log($"ステージ {stageData.stageID} 失敗: {stageData.dialogueSet.dialogueFail}");
    }
    
    // エンディングの演出
    public void PresentEndingDialogue()
    {
        Debug.Log($"ステージ {stageData.stageID} エンディング: {stageData.dialogueSet.endingDialogue}");
    }
}
