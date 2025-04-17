using UnityEngine;
using VContainer;

[RequireComponent(typeof(Collider2D))]
public class Actor : MonoBehaviour
{
    public Vector2 velocity;

    GravityField _gravityField;

    public void SetGravityField(GravityField gravityField)
    {
        _gravityField = gravityField;
    }

    public void UpdateActor(float deltaTime)
    {
        // 中心重力
            velocity += _gravityField.GetForce(transform.position) * deltaTime;

        transform.position += (Vector3)(velocity * deltaTime);
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