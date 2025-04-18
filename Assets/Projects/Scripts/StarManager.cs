using System.Collections.Generic;
using R3;

public class StarManager : Presenter
{
    public bool AreAllStarsCollected => obtainedCount == starCountTotal;
    
    private int starCountTotal;
    int obtainedCount;

    public StarManager(Star[] stars, GlobalMessage globalMessage)
    {
        starCountTotal = stars.Length;

        foreach (var item in stars)
        {
            AddDisposable(item.OnHit.Subscribe(x =>
            {
                EnsinLog.Info($"starCountTotal {obtainedCount}, {starCountTotal}");

                obtainedCount++;
            }));
        }
    }
}