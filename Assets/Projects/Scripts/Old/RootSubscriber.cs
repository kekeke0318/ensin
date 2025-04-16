using MessagePipe;
using UnityEngine;
using VContainer;

public class RootSubscriber
{
    [Inject] public ISubscriber<Signals.GameOver> onGameOver;
}
