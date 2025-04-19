using VContainer;
using Cysharp.Threading.Tasks;
using R3;
using MessagePipe;
using VContainer.Unity;

public class DialogueEntryPoint : Presenter, IInitializable
{
[Inject] CancellationTokenProvider _ctProvider;
[Inject] ActorMessageView _actorMessageView;

    private bool _isFirstStarHit = false;

    public void Initialize()
    {
    }

    public DialogueEntryPoint(GlobalMessage globalMessage)
    {
            EnsinLog.Info("DialoguePresenter");

        AddDisposable(globalMessage.hitStarSub.Subscribe(x =>
        {
            EnsinLog.Info("スターを取得");

            if (!_isFirstStarHit)
            {
                _isFirstStarHit = true;
                _ = PresentStarGetDialogue();
            }
        }));
    }

    public async UniTask PresentStarGetDialogue()
    {
        EnsinLog.Info("スターを初めて取得した時の会話");
        // UI表示やアニメーションもここで

        _actorMessageView.SetText($"うわあ");

        await UniTask.WaitForSeconds(1, cancellationToken: _ctProvider.Token);
    }
}