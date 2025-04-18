using MessagePipe;
using UnityEngine;
using VContainer;

public class GlobalMessage
{
   [Inject] public ISubscriber<ActorLaunchedEvent> actorLaunchedSub;
   [Inject] public ISubscriber<HitStarEvent> hitStarSub;
   [Inject] public ISubscriber<RetryEvent> retryRequestedSub;

   [Inject] public IPublisher<ActorLaunchedEvent> actorLaunchedPub;
   [Inject] public IPublisher<HitStarEvent> hitStarPub;
   [Inject] public IPublisher<RetryEvent> retryRequestedPub;
}
