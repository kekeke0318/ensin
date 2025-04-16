using System.Collections.Generic;

public class StarManager
{
    public List<Star> stars = new List<Star>();

    // 全 Star 取得済みか判定
    public bool AreAllStarsCollected()
    {
        foreach (var star in stars)
        {
            if (!star.isCollected)
                return false;
        }
        return true;
    }
}