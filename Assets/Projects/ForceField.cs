using UnityEngine;

public class ForceField : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] private float _radius = 5f;   // 重力を及ぼす範囲
    [SerializeField] private float _power = 1f;    // 引き寄せる強さ

    Mover _mover;
    
    public void SetMover(Mover target)
    {
        _mover = target;
    }

    private void Update()
    {
       
    }

    private void OnDrawGizmos()
    {
    }
}