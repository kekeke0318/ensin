// Assets/Projects/Scripts/Mother.cs
// ------------------------------------------------------------
// Mother: ステージに 1 体だけ存在し、Actor を発射するキャラクター
// ※ 現状のアーキテクチャに合わせ、発射処理は GlobalMessage 経由で
//    ActorManager へ通知し、ActorFactory が生成します。
//    GameLifetimeScope の Factory 内で生成位置を Mother.CurrentPosition
//    に変更すると、Mother の座標から正しく Actor がスポーンします。
// ------------------------------------------------------------

using System;
using UnityEngine;
using MessagePipe;
using VContainer;
using VContainer.Unity;

public interface ICameraTarget
{
    Transform Transform { get; }
}

public class MotherView : MonoBehaviour, ICameraTarget
{
    [SerializeField] Animator _anim;

    public Transform Transform { get; private set; }
    public Vector2 Position => transform.position;
    public Animator Anim => _anim;

    void Awake()
    {
        Transform = transform;
    }

#if UNITY_EDITOR
    // エディタ上で Mother の位置を可視化
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, 0.15f);
    }
#endif
}
