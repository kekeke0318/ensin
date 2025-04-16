using VContainer;
using VContainer.Unity;
using MessagePipe;
using UnityEngine;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField]
    private StageData stageDataAsset; // エディタ上で各ステージの設定データをセットする

    protected override void Configure(IContainerBuilder builder)
    {
        // MessagePipe の初期設定
        var options = builder.RegisterMessagePipe();
        options.EnableCaptureStackTrace = true;

        // ピュアな Manager クラスをシングルトンとして登録
        builder.Register<ActorManager>(Lifetime.Singleton); // ActorManager の実装は前回の通り
        builder.Register<StarManager>(Lifetime.Singleton);

        // MotherPresenter をピュアなクラスとして登録。ステージデータを渡して初期化
        builder.RegisterInstance(new MotherPresenter(stageDataAsset))
            .AsSelf();

        // StageManager を登録。StarManager と MotherPresenter、そして同じステージデータを注入
        builder.Register<StageManager>(Lifetime.Singleton);

        // GameEntryPoint 等のエントリーポイントは別途登録する
        builder.RegisterEntryPoint<GameEntryPoint>();

        Star[] stars =
            this.transform.GetComponentsInChildren<Star>(includeInactive: true);

        //配列をそのまま一つのインスタンスとして登録
        builder.RegisterInstance(stars)
            .AsSelf();

        builder.RegisterBuildCallback(x => { GlobalMessagePipe.SetProvider(x.AsServiceProvider()); });
    }
}