using UnityEngine;

/// <summary>
/// 指定方向へ一定の「加速度」を与えるフィールド。
/// BoxCollider2D をトリガーにしておくこと。
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class CircleGravityField : MonoBehaviour
{
    public Vector3 Position => _t.position;
    
    [SerializeField] float _power = 1f;

    CircleCollider2D _col;
    Transform _t;

    void Awake()
    {
        _t = transform;
        _col = GetComponent<CircleCollider2D>();
        _col.isTrigger = true;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var actor = other.GetComponent<ActorView>();
        if (actor == null) return;

        // 速度に積分して「加速」させる
        actor.AddVelocity((transform.position - other.transform.position) * _power * Time.deltaTime);
    }
}