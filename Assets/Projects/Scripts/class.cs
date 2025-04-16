using UnityEngine;

public static class ActorFactory
{
    public static Actor CreateActor(Vector2 launchVector)
    {
        GameObject prefab = Resources.Load<GameObject>("ActorPrefab");
        GameObject obj = UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        Actor actor = obj.GetComponent<Actor>();
        actor.velocity = launchVector;
        return actor;
    }
}
