using UnityEngine;
using System.Collections.Generic;

public class Star
{
    public bool isCollected = false;
    
    public void Collect()
    {
        isCollected = true;
        // エフェクト等の演出処理を追加可能
    }
}
