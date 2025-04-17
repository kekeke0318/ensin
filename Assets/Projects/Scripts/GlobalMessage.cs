using MessagePipe;
using UnityEngine;
using VContainer;

public class GlobalMessage
{
   [Inject] public ISubscriber<ActorLaunchedEvent> actorLaunchedSub;
   [Inject] public ISubscriber<HitStarEvent> hitStarSub;
   
   [Inject] public IPublisher<ActorLaunchedEvent> actorLaunchedPub;
   [Inject] public IPublisher<HitStarEvent> hitStarPub;
}
