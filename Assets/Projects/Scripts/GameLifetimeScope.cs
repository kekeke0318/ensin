using VContainer;
using VContainer.Unity;
using MessagePipe;
using UnityEngine;
using System.Threading;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private StageData stageDataAsset;

    protected override void Configure(IContainerBuilder builder)
    {
        // MessagePipe の初期設定
        var options = builder.RegisterMessagePipe();
        
        builder.RegisterInstance(stageDataAsset);
        
        // ピュアな Manager クラスをシングルトンとして登録
        builder.Register<GlobalFactory>(Lifetime.Singleton);
        builder.Register<GlobalMessage>(Lifetime.Singleton);
        builder.Register<StageManager>(Lifetime.Singleton);
        builder.Register<StageRetryUseCase>(Lifetime.Singleton);
        builder.Register<CancellationTokenProvider>(Lifetime.Singleton).WithParameter(new CancellationTokenSource());
        builder.Register<MotherPresenter>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<StarPresenter>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<ActorManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<RetryFxPresenter>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

        // Hierarchy
        builder.RegisterComponentInHierarchy<InputController>();
        builder.RegisterComponentInHierarchy<TrajectoryLineView>();
        builder.RegisterComponentInHierarchy<MainCameraView>();
        builder.RegisterComponentInHierarchy<MotherView>();
        builder.RegisterComponentInHierarchy<ActorMessageView>();
        
        // GameEntryPoint 等のエントリーポイントは別途登録する
        builder.RegisterEntryPoint<DialogueEntryPoint>();
        builder.RegisterEntryPoint<GameEntryPoint>();

        Star[] stars = FindObjectsByType<Star>(FindObjectsSortMode.None);

        //配列をそのまま一つのインスタンスとして登録
        builder.RegisterInstance(stars)
            .AsSelf();
        
        builder.RegisterFactory<Vector2, ActorView>(container => (launchVector) =>
            {
                var actor = Instantiate(stageDataAsset.actorViewPrefab, Vector2.zero, Quaternion.identity);
                actor.SetVelocity(launchVector);
                return actor;
            },
            Lifetime.Singleton);
    }
}

public class CancellationTokenProvider
{
    public CancellationToken Token { get; }
    public CancellationTokenProvider(CancellationTokenSource cst) => Token = cst.Token;
}