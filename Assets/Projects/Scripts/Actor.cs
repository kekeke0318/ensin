using UnityEngine;
using VContainer;

[RequireComponent(typeof(Collider2D))]
public class Actor : MonoBehaviour
{
    public Vector2 velocity;

    private const float Y_GRAVITY = -9.8f;   // 下向き重力
    GravityField _gravityField;

    public void SetGravityField(GravityField gravityField)
    {
        _gravityField = gravityField;
    }

    public void UpdateActor(float deltaTime)
    {
        // 下向き重力
        velocity.y += Y_GRAVITY * deltaTime;

        // 中心重力
        if (_gravityField != null)
            velocity += _gravityField.GetForce(transform.position) * deltaTime;

        transform.position += (Vector3)(velocity * deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var star = other.GetComponent<Star>();
        if (star != null) star.Collect();
    }
}