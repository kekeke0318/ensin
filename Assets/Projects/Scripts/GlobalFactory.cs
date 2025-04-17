using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class GlobalFactory
{
    [Inject] System.Func<Vector2, Actor> _actorFactory;

    public Actor CreateActor(Vector2 launchVector)
    {
        return _actorFactory(launchVector);
    }
}
