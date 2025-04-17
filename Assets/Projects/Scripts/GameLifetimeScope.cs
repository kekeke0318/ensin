using VContainer;
using VContainer.Unity;
using MessagePipe;
using UnityEngine;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private StageData stageDataAsset;

    protected override void Configure(IContainerBuilder builder)
    {
        // MessagePipe の初期設定
        var options = builder.RegisterMessagePipe();
        
        builder.RegisterInstance(stageDataAsset);
        
        // ピュアな Manager クラスをシングルトンとして登録
        builder.Register<InputService>(Lifetime.Transient);
        builder.Register<GlobalFactory>(Lifetime.Singleton);
        builder.Register<GlobalMessage>(Lifetime.Singleton);
        builder.Register<ActorManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<StarManager>(Lifetime.Singleton);
        builder.Register<MotherPresenter>(Lifetime.Singleton);
        builder.Register<StageManager>(Lifetime.Singleton);
        
        // Hierarchy
        builder.RegisterComponentInHierarchy<TrajectoryLineView>();
        builder.RegisterComponentInHierarchy<MainCameraView>();
        builder.RegisterComponentInHierarchy<MotherView>();

        // GameEntryPoint 等のエントリーポイントは別途登録する
        builder.RegisterEntryPoint<GameEntryPoint>();

        Star[] stars =
            this.transform.GetComponentsInChildren<Star>(includeInactive: true);

        //配列をそのまま一つのインスタンスとして登録
        builder.RegisterInstance(stars)
            .AsSelf();
        
        builder.RegisterFactory<Vector2, Actor>(container => (launchVector) =>
            {
                var actor = Instantiate(stageDataAsset.actorPrefab, Vector2.zero, Quaternion.identity);
                actor.SetVelocity(launchVector);
                return actor;
            },
            Lifetime.Singleton);
    }
}