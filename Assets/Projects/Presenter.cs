using MessagePipe;
using R3;
using UnityEngine;
using VContainer;

public class Presenter : System.IDisposable
{
    [Inject] protected RootSubscriber rootSubscriber;

    CompositeDisposable _compositeDisposable = new CompositeDisposable();
    
    public void SubscribeOnGameOver(System.Action<Signals.GameOver> action)
    {
        _compositeDisposable.Add(rootSubscriber.onGameOver.Subscribe(action));
    }
    
    public void Dispose()
    {
        _compositeDisposable.Clear();
    }
}
