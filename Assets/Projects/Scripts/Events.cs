using UnityEngine;

public struct ActorLaunchedEvent
{
    public Vector2 LaunchVector { get; set; }
    public Vector2 Position { get; set; }
}

public struct HitStarEvent
{
}

public struct RetryEvent
{
}