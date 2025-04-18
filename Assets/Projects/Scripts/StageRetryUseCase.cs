using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 「演出が終わったら同じシーンをロードし直す」だけの UseCase
/// </summary>
public sealed class StageRetryUseCase
{
    readonly RetryFxPresenter _fx;

    public StageRetryUseCase(RetryFxPresenter fx) => _fx = fx;

    public async UniTask RetryAsync()
    {
        await _fx.PlayAsync();   // 演出完了まで待機
        await SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}