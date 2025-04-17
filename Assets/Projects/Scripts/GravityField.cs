using UnityEngine;

/// <summary>
/// シーン中心に向かう円形重力フィールド
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class GravityField : MonoBehaviour
{
    [SerializeField] float gravityStrength = 9.8f;
    [SerializeField] float radius = 5f;

    CircleCollider2D col;

    void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = radius;
    }

    /// <param name="actorPos">対象座標</param>
    /// <param name="mass">質量（加速度 = F / m）</param>
    public Vector2 GetForce(Vector2 actorPos, float mass = 1f)
    {
        Vector2 dir = (Vector2)transform.position - actorPos;
        if (dir.magnitude > radius) return Vector2.zero;

        return dir.normalized * gravityStrength * mass;
    }

    public float Radius => radius;
}
