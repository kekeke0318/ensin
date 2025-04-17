using System;
using System.Collections.Generic;
using MessagePipe;
using VContainer;
using VContainer.Unity;

public class ActorManager : IInitializable, System.IDisposable
{
    [Inject] GlobalFactory _globalFactory; 
    [Inject] GlobalMessage _globalMessage; 
    
    public List<Actor> actors = new List<Actor>();

    private IDisposable _disposable;

    // コンストラクタで GlobalMessagePipe を利用してイベント購読
    public ActorManager()
    {
    }

    private void OnActorLaunched(ActorLaunchedEvent e)
    {
        // 発射イベント受信時、ActorFactory を利用して Actor を生成
        var actor = _globalFactory.CreateActor(e.LaunchVector);
        actors.Add(actor);
    }

    // ゲームループ内から定期的に呼ばれる更新処理
    public void Update(float deltaTime)
    {
        foreach (var actor in actors)
        {
            actor.UpdateActor(deltaTime);
        }
    }

    public void Initialize()
    {
        var bag = DisposableBag.CreateBuilder();
        _globalMessage.actorLaunchedSub.Subscribe(OnActorLaunched).AddTo(bag);
        _disposable = bag.Build();
    }

    public void Dispose()
    {
        _disposable?.Dispose();
    }
}