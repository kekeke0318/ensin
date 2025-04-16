namespace Project
{
using UnityEngine;
using System.Collections.Generic;

public class BulletTrajectoryPredictor
{
    private readonly float simulationStep = 0.02f; // UnityのFixedUpdateに相当
    private readonly int simulationSteps = 300; // 約6秒分のシミュレーション（300 × 0.02）

    private float bulletMass;
    private float bulletPower;

    public BulletTrajectoryPredictor(float mass, float power)
    {
        bulletMass = mass;
        bulletPower = power;
    }

    public List<Vector2> PredictTrajectory(Vector2 initialPosition, Vector2 initialVelocity)
    {
        List<Vector2> trajectoryPoints = new List<Vector2>();
        Vector2 position = initialPosition;
        Vector2 velocity = initialVelocity;

        for (int i = 0; i < simulationSteps; i++)
        {
            // 毎フレームの力を計算 (-position * power)
            Vector2 force = -position * bulletPower;

            // 加速度計算
            Vector2 acceleration = force / bulletMass;

            // 速度と位置の更新 (オイラー積分)
            velocity += acceleration * simulationStep;
            position += velocity * simulationStep;

            trajectoryPoints.Add(position);
        }

        return trajectoryPoints;
    }

    // 衝突予測メソッド
    public bool WillCollide(List<Vector2> trajectory, List<List<Vector2>> otherTrajectories, float collisionRadius)
    {
        for (int step = 0; step < trajectory.Count; step++)
        {
            foreach (var otherTrajectory in otherTrajectories)
            {
                if (step >= otherTrajectory.Count) continue;

                float distance = Vector2.Distance(trajectory[step], otherTrajectory[step]);
                if (distance <= collisionRadius * 2) // Bullet同士の衝突距離
                {
                    return true; // 衝突予測あり
                }
            }
        }

        return false; // 衝突なし
    }
}

}