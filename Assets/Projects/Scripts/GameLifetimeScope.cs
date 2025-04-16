using VContainer;
using VContainer.Unity;
using MessagePipe;
using UnityEngine;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] 
    private StageData stageDataAsset;  // エディタ上で各ステージデータを設定できるようにする
    
    protected override void Configure(IContainerBuilder builder)
    {
        // MessagePipe のセットアップ（GlobalMessagePipe 経由でイベントの発行／購読を行う）
        builder.RegisterMessagePipe();

        builder.RegisterEntryPoint<GameEntryPoint>();
        
        // 通常クラスとして各 Manager をシングルトンで登録
        builder.Register<ActorManager>(Lifetime.Singleton);
        builder.Register<StarManager>(Lifetime.Singleton);
        
        // MotherPresenter をシングルトンとして登録。ここでステージデータを DI する
        builder.RegisterInstance(new MotherPresenter(stageDataAsset))
            .AsSelf()
            .AsImplementedInterfaces();  // 必要ならインターフェースとしても登録
    }
}