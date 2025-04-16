using System;
using System.Collections.Generic;
using MessagePipe;

public class ActorManager
{
    public List<Actor> actors = new List<Actor>();

    private IDisposable launchEventSubscription;

    // コンストラクタで GlobalMessagePipe を利用してイベント購読
    public ActorManager()
    {
        var subscriber = GlobalMessagePipe.GetSubscriber<ActorLaunchedEvent>();
        launchEventSubscription = subscriber.Subscribe(OnActorLaunched);
    }

    private void OnActorLaunched(ActorLaunchedEvent e)
    {
        // 発射イベント受信時、ActorFactory を利用して Actor を生成
        var actor = ActorFactory.CreateActor(e.LaunchVector);
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
}