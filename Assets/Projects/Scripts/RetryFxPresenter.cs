using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using VContainer; // DI
using MessagePipe; // 演出呼び出し用メッセージにも対応させるなら

/// <summary>
/// Mother を瞬時に Actor の位置へワープさせて抱っこ→フェードアウトする演出
/// </summary>
public sealed class RetryFxPresenter : Presenter
{
    [Inject] MainCameraView _mainCamera;
    [Inject] MotherView _motherView;
    [Inject] ActorManager _actorManager;
    [SerializeField] CanvasGroup _fadeCanvas; // 真っ黒で α=0 の UI

    static readonly int Hug = Animator.StringToHash("Hug");

    public RetryFxPresenter(GlobalMessage globalMessage)
    {
    }

    public async UniTask PlayAsync(CancellationToken ct)
    {
        _mainCamera.SetTarget(_motherView);
        
        // ① ワープ
        _motherView.transform.position = _actorManager.ActorView.Position;

        await _motherView.transform.DOMove(_actorManager.ActorView.Position, 1);

        // ② 抱っこアニメ
        var anim = _motherView.Anim;
        anim.ResetTrigger(Hug);
        anim.SetTrigger(Hug);

        // AnimatorUtility 拡張（下に定義）で終了待ち
        await anim.WaitStateExitAsync("Hug", ct);

        // ③ フェードアウト
        await _fadeCanvas.DOFade(1, 0.25f).SetEase(Ease.InQuad);
    }
}

/// <summary>Animator ステート終了待ちの簡易実装（UniTask 0.60+ 相当）</summary>
static class AnimatorUtility
{
    public static async UniTask WaitStateExitAsync(this Animator anim, string stateName,
        CancellationToken token)
    {
        int layer = 0;
        while (!token.IsCancellationRequested)
        {
            if (anim.GetCurrentAnimatorStateInfo(layer).IsName(stateName) == false) break;
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }
}