using System;
using System.Collections.Generic;
using MessagePipe;
using VContainer;
using VContainer.Unity;
using R3;

public class ActorManager : Presenter, IInitializable
{
    [Inject] StageData _stageData;
    [Inject] GlobalFactory _globalFactory;
    [Inject] GlobalMessage _globalMessage;
    [Inject] ActorMessageView _actorMessageView;

    public ActorView ActorView { get; private set; }

    private IDisposable _disposable;

    public ActorManager()
    {
    }

    private void OnActorLaunched(ActorLaunchedEvent e)
    {
        if (ActorView != null) return;

        // 発射イベント受信時、ActorFactory を利用して Actor を生成
        ActorView = _globalFactory.CreateActor(e.LaunchVector);
        AddDisposable(ActorView.OnHit.Subscribe(OnActorHit));
        ActorView.SetPosition(e.Position);
    }

    private void OnActorHit(Unit unit)
    {
        EnsinLog.Info($"OnActorHit");
        _globalMessage.hitStarPub.Publish(new HitStarEvent());
    }

    // ゲームループ内から定期的に呼ばれる更新処理
    public void Update(float deltaTime)
    {
        ActorView.UpdateActor(deltaTime);
    }

    public void Initialize()
    {
        var bag = MessagePipe.DisposableBag.CreateBuilder();
        _globalMessage.actorLaunchedSub.Subscribe(OnActorLaunched).AddTo(bag);
        _disposable = bag.Build();
    }

    public void Dispose()
    {
        _disposable?.Dispose();
    }
}