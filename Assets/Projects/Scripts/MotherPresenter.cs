using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;
using VContainer;

public class MotherPresenter : Presenter
{
    GlobalMessage _globalMessage;
    
    private readonly StageData stageData;
    
    // コンストラクタで対応するステージデータを DI 経由などで受け取る
    public MotherPresenter(StageData stageData, GlobalMessage globalMessage)
    {
        this.stageData = stageData;
        _globalMessage = globalMessage;
        
        AddDisposable(_globalMessage.actorLaunchedSub.Subscribe(x =>
        {
        }));
    }
    
    // クリア時の会話演出を実行
    public async UniTask PresentClearDialogue()
    {
        // 例：UI にテキストを表示するなどの処理（ここでは Debug.Log で簡略化）
        Debug.Log($"ステージ {stageData.stageID} クリア: {stageData.dialogueSet.dialogueClear}");
        
        // DOTween を用いたアニメーション演出などの実装もここで行う
    }
    
    // 失敗時の会話演出を実行
    public async UniTask PresentFailDialogue()
    {
        Debug.Log($"ステージ {stageData.stageID} 失敗: {stageData.dialogueSet.dialogueFail}");
    }
    
    // エンディングの演出
    public async UniTask PresentEndingDialogue()
    {
        Debug.Log($"ステージ {stageData.stageID} エンディング: {stageData.dialogueSet.endingDialogue}");
    }

    public async UniTask PresentIntroDialogue()
    {
        Debug.Log($"ステージ {stageData.stageID} Intro: {stageData.dialogueSet.endingDialogue}");
    }
}
