using System;
using UnityEngine;

public class TrajectoryLineView : MonoBehaviour
{
    [SerializeField] LineRenderer _renderer;

    public void SetPositionCount(int count)
    {
        _renderer.positionCount = count;
    }

    public void SetPosition(int i, Vector2 point)
    {
        _renderer.SetPosition(i, point);
    }
}
