using System;
using UnityEngine;
using VContainer;
using R3;

[RequireComponent(typeof(Collider2D))]
public class ActorView : MonoBehaviour, ICameraTarget
{
    public Observable<Unit> OnHit => _onHit;
    public Transform Transform { get; private set; }
    public Vector2 Position => transform.position;

    Subject<Unit> _onHit = new Subject<Unit>();

    Vector2 _velocity;

    void Awake()
    {
        Transform = transform;
    }

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
        if (star != null)
        {
             star.Collect();
            _onHit.OnNext(Unit.Default);
        }
    }

    public void SetPosition(Vector2 pos)
    {
        transform.position = pos;
    }
}