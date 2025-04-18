using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class StageManager
{
    private readonly StarManager starManager;
    private readonly MotherPresenter motherPresenter;
    private readonly StageData stageData;
    
    public StageManager(StarManager starManager, MotherPresenter motherPresenter, StageData stageData)
    {
        this.starManager = starManager;
        this.motherPresenter = motherPresenter;
        this.stageData = stageData;
    }
    
    // ステージの結果評価（非同期処理例）
    public async UniTask EvaluateStageAsync(CancellationToken ct)
    {
        // StarManager 側で全 Star の取得を判定
        bool allCollected = starManager.AreAllStarsCollected;
        
        // 結果判定前に少し待機（演出のための余裕時間など）
        await UniTask.Delay(500, cancellationToken: ct);
        
        if (allCollected)
        {
            await motherPresenter.PresentClearDialogue();
        }
        else
        {
            await motherPresenter.PresentFailDialogue();
        }
    }
}
