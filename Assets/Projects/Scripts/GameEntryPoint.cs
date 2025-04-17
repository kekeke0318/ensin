using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

public class GameEntryPoint : IAsyncStartable
{
    // DI で取得した ActorManager、StarManager はシングルトンから取得可能（GlobalMessagePipe 経由や自前のシングルトン管理）
    // ここでは簡単のため、LifetimeScope で登録された ActorManager を事前に初期化済みと仮定

    [Inject] InputService _inputService;
    [Inject] ActorManager _actorManager;
    [Inject] StarManager _starManager;
    [Inject] GlobalFactory _globalFactory;

    public async UniTask StartAsync(CancellationToken ct)
    {
        // シンプルな更新ループ例（実際は UniTask やコルーチンで実装）
        while (true)
        {
            // ここでイベントを受け取っていたらアクター生成
            // if文を使わずUniRxもしくはUniTaskで実現したい
            
            float dt = Time.deltaTime;
            _inputService.Update();
            _actorManager.Update(dt);
            // starManager.Update(dt); // 必要なら更新処理を呼ぶ
            await UniTask.Yield();
        }
    }
}