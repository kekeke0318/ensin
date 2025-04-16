using System.Collections.Generic;

public class StarManager
{
    List<Star> _stars = new List<Star>();

    public StarManager(Star[] stars)
    {
        _stars.AddRange(stars);
    }

    // 全 Star 取得済みか判定
    public bool AreAllStarsCollected()
    {
        foreach (var star in _stars)
        {
            if (!star.isCollected)
                return false;
        }
        return true;
    }
}