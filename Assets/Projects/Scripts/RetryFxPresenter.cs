using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using VContainer; // DI
using MessagePipe; // 演出呼び出し用メッセージにも対応させるなら

/// <summary>
/// Mother を瞬時に Actor の位置へワープさせて抱っこ→フェードアウトする演出
/// </summary>
public sealed class RetryFxPresenter : MonoBehaviour
{
    [SerializeField] MotherView mother;
    [SerializeField] ActorView actorView;
    [SerializeField] CanvasGroup fadeCanvas; // 真っ黒で α=0 の UI

    static readonly int Hug = Animator.StringToHash("Hug");

    public async UniTask PlayAsync()
    {
        // ① ワープ
        mother.transform.position = actorView.transform.position;

        // ② 抱っこアニメ
        var anim = mother.Anim;
        anim.ResetTrigger(Hug);
        anim.SetTrigger(Hug);

        // AnimatorUtility 拡張（下に定義）で終了待ち
        await anim.WaitStateExitAsync("Hug", this.GetCancellationTokenOnDestroy());

        // ③ フェードアウト
        await fadeCanvas.DOFade(1, 0.25f).SetEase(Ease.InQuad);
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