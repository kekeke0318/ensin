using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using MessagePipe;

public class StarManager : Presenter
{
    public bool AreAllStarsCollected => obtainedCount == starCountTotal;
    
    private int starCountTotal;
    int obtainedCount;

    public StarManager(Star[] stars, GlobalMessage globalMessage)
    {
        starCountTotal = stars.Length;
        EnsinLog.Info($"starCountTotal {starCountTotal}");
        
        globalMessage.hitStarSub.Subscribe(e =>
        {
            obtainedCount++;
            if (obtainedCount >= starCountTotal)
            {
                OnAllStarsObtained();
            }
        });
    }

    private void OnAllStarsObtained()
    {
        // ここでクリアイベントや遷移をPublishしたり、GameEntryPointに伝える
        Debug.Log("All Stars Obtained! Game Clear!");
    }
}