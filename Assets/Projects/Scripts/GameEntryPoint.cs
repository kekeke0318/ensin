using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using R3;
using DisposableBag = MessagePipe.DisposableBag;

public sealed class GameEntryPoint : IInitializable, IAsyncStartable, IDisposable
{
    // 依存
    [Inject] ActorManager _actorMgr;
    [Inject] StarManager _starMgr;
    [Inject] MainCameraView _mainCamera;
    [Inject] StageManager _stageMgr;
    [Inject] MotherPresenter _mother;
    [Inject] MotherView _motherView;
    [Inject] TrajectoryLineView _trajectoryLine;
    [Inject] GameData _gameData;
    [Inject] StageData _stageData;
    [Inject] GlobalMessage _msg;
    [Inject] StageRetryUseCase _retryUseCase;
    [Inject] InputController _input;

    // disposable
    IDisposable _bag;

    public void Initialize()
    {
        _input.SetTrajectoryLine(_trajectoryLine);
        _input.SetCamera(_mainCamera.Cam);
    }

    public async UniTask StartAsync(CancellationToken ct)
    {

        var bag = DisposableBag.CreateBuilder();

        // ── Intro ─────────────────────────────────────────
        await _mother.PresentIntroDialogue(); // 会話開始

        // ── Aiming ────────────────────────────────────────
        _input.SetDragStartPos(_motherView.Position);
        _input.OnShootInput.Subscribe(_msg.actorLaunchedPub.Publish).AddTo(ct);
        await _msg.actorLaunchedSub.FirstAsync(ct); // 発射を待つ

        // ── Flying ────────────────────────────────────────
        bool retryRequested = false;
        var retryDisposable = _msg.retryRequestedSub.Subscribe(_ => retryRequested = true);
        retryDisposable.AddTo(bag);
        bag.Add(_input.OnRetryInput.Subscribe(_msg.retryRequestedPub.Publish));

        // Star 全取得 or リトライ要求まで毎フレーム更新
        while (!retryRequested && !_starMgr.AreAllStarsCollected)
        {
            float dt = Time.deltaTime;
            _actorMgr.Update(dt);
            await UniTask.Yield(ct);
        }

        if (retryRequested)
        {
            retryDisposable.Dispose();
            
            // 演出付きリトライを非同期 fire‑and‑forget
            await _retryUseCase.RetryAsync(ct);
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