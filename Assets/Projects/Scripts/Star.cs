using UnityEngine;

public class Star : MonoBehaviour
{
    public bool isCollected { get; private set; } = false;

    public void Collect()
    {
        if (isCollected) return;
        isCollected = true;

        // 演出やエフェクト
        Debug.Log($"Star collected: {gameObject.name}");

        // オプション：見た目を非表示にするなど
        gameObject.SetActive(false);
    }
}