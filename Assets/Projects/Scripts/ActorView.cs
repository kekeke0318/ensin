using UnityEngine;
using VContainer;

[RequireComponent(typeof(Collider2D))]
public class ActorView : MonoBehaviour
{
    Vector2 _velocity;

    public void AddVelocity(Vector2 velocity)
    {
        _velocity += velocity;
    }
    
    public void SetVelocity(Vector2 velocity)
    {
        _velocity = velocity;
    }
    
    public void UpdateActor(float deltaTime)
    {
        transform.position += (Vector3)(_velocity * deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var star = other.GetComponent<Star>();
        if (star != null) star.Collect();
    }

    public void SetPosition(Vector2 pos)
    {
        transform.position = pos;
    }
}