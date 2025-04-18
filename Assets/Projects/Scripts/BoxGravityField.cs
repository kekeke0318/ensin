using Cysharp.Threading.Tasks;
using R3.Triggers;
using R3;
using UnityEngine;

/// <summary>
/// 指定方向へ一定の「加速度」を与えるフィールド。
/// BoxCollider2D をトリガーにしておくこと。
/// </summary>
public class BoxGravityField : MonoBehaviour
{
    [SerializeField] Vector2 acceleration = new Vector2(8f, 0f);
    [SerializeField] GameObject _triggerObject;

    Transform _t;

    void Awake()
    {
        _t = transform;
        _triggerObject.OnTriggerStay2DAsObservable().Subscribe(other =>
        {
            var actor = other.GetComponent<ActorView>();
            if (actor == null) return;

            // 速度に積分して「加速」させる
            actor.AddVelocity(acceleration * Time.deltaTime);
        }).AddTo(this);
    }
}