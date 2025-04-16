using UnityEngine;

public class Actor : MonoBehaviour
{
    public Vector2 velocity;
    private float gravity = -9.8f;

    public void UpdateActor(float deltaTime)
    {
        velocity.y += gravity * deltaTime;
        transform.position += (Vector3)(velocity * deltaTime);
    }
}