using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;
using MessagePipe;

public sealed class GameEntryPoint : IAsyncStartable, IDisposable
{
    // 依存
    [Inject] InputService _input;
    [Inject] ActorManager _actorMgr;
    [Inject] StarManager _starMgr;
    [Inject] StageManager _stageMgr;
    [Inject] MotherPresenter _mother;
    [Inject] GameData _gameData;
    [Inject] StageData _stageData;
    [Inject] GlobalMessage _msg;
    [Inject] StageRetryUseCase _retryUseCase;

    // disposable
    IDisposable _bag;

    public async UniTask StartAsync(CancellationToken ct)
    {
        var bag = DisposableBag.CreateBuilder();

        // ── Intro ─────────────────────────────────────────
        _input.SetEnabled(false);
        await _mother.PresentIntroDialogue(); // 会話開始

        // ── Aiming ────────────────────────────────────────
        _input.SetEnabled(true);
        await _msg.actorLaunchedSub.FirstAsync(ct); // 発射を待つ

        // ── Flying ────────────────────────────────────────
        _input.SetEnabled(false);

        bool retryRequested = false;
        _msg.retryRequestedSub.Subscribe(_ => retryRequested = true).AddTo(bag);

        // Star 全取得 or リトライ要求まで毎フレーム更新
        while (!retryRequested && !_starMgr.AreAllStarsCollected)
        {
            float dt = Time.deltaTime;
            _actorMgr.Update(dt);
            await UniTask.Yield(ct);
        }

        if (retryRequested)
        {
            // 演出付きリトライを非同期 fire‑and‑forget
            await _retryUseCase.RetryAsync();
        }

        // ── Result ────────────────────────────────────────
        await _stageMgr.EvaluateStageAsync(ct);

        // ── Next / Retry ──────────────────────────────────
        string nextScene = SceneManager.GetActiveScene().name;
        if (_starMgr.AreAllStarsCollected)
        {
            if (_stageData.isLast)
            {
                nextScene = $"{_gameData.endingSceneName}";
            }
            else
            {
                nextScene = $"Stage_{_stageData.stageID + 1}";
            }
        }

        await SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);
        _bag = bag.Build();
    }

    public void Dispose() => _bag?.Dispose();
}