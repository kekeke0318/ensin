using System.Collections.Generic;
using R3;

public class StarPresenter : Presenter
{
    public bool AreAllStarsCollected => obtainedCount == starCountTotal;

    private int starCountTotal;
    int obtainedCount;

    public StarPresenter(Star[] stars, GlobalMessage globalMessage)
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