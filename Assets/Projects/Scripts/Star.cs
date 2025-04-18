using System;
using R3;
using UnityEngine;

public class Star : MonoBehaviour
{
    public Observable<Unit> OnHit => _onHit;
    public bool isCollected { get; private set; } = false;

    Subject<Unit> _onHit = new ();

    public void Collect()
    {
        if (isCollected) return;
        isCollected = true;

        // 演出やエフェクト
        Debug.Log($"Star collected: {gameObject.name}");

        // オプション：見た目を非表示にするなど
        gameObject.SetActive(false);
        
        _onHit.OnNext(Unit.Default);
    }
}