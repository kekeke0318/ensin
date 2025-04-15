using UnityEngine;

[CreateAssetMenu(menuName = "Settings/PhysicsSimulationSettings")]
public class PhysicsSimulationSettings : ScriptableObject {
    public float stepTime = 0.02f;
    public float gravitationalPower = 1f;
}