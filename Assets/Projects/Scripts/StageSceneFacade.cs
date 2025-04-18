using System;
using UnityEngine;

public class StageSceneFacade : MonoBehaviour
{
    public TrajectoryLineView TrajectoryLine { get; private set; }

    void Awake()
    {
        TrajectoryLine = GetComponentInChildren<TrajectoryLineView>();
    }
}
