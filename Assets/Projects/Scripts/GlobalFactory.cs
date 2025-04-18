using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class GlobalFactory
{
    [Inject] System.Func<Vector2, ActorView> _actorFactory;

    public ActorView CreateActor(Vector2 launchVector)
    {
        return _actorFactory(launchVector);
    }
}
