using UnityEngine;
using System.Collections.Generic;

public class TrajectoryAssist {
    private PhysicsSimulationSettings settings;

    public bool enableSafeTrajectory = true;
    public int simulationSteps = 300;
    public float angleStep = 5f;
    public int maxAttempts = 10;

    // 共通設定をセット
    public void Initialize(PhysicsSimulationSettings simulationSettings) {
        settings = simulationSettings;
    }

    // ユーザーの弾の軌道をシンプルなオイラー法でシミュレーション
    public List<Vector2> SimulateTrajectory(Vector2 position, Vector2 velocity) {
        List<Vector2> points = new List<Vector2>();
        Vector2 pos = position;
        Vector2 vel = velocity;
        for (int i = 0; i < simulationSteps; i++){
            // 設定から重力パワーを参照（中心（原点）に向かう加速度）
            Vector2 acceleration = -pos * settings.gravitationalPower;
            vel += acceleration * settings.stepTime;
            pos += vel * settings.stepTime;
            points.Add(pos);
        }
        return points;
    }

    // 新規弾の予測軌道 newTrajectory とその半径 newBulletRadius、
    // 既存の弾の予測軌道と半径のタプルリスト otherTrajectories を用いた衝突判定
    public bool WillCollide(List<Vector2> newTrajectory, float newBulletRadius, List<(List<Vector2> trajectory, float radius)> otherTrajectories) {
        int steps = Mathf.Min(newTrajectory.Count, simulationSteps);
        foreach (var tuple in otherTrajectories) {
            List<Vector2> otherTrajectory = tuple.trajectory;
            float otherRadius = tuple.radius;
            int otherSteps = Mathf.Min(otherTrajectory.Count, simulationSteps);
            int minSteps = Mathf.Min(steps, otherSteps);
            for (int i = 0; i < minSteps; i++) {
                float dist = Vector2.Distance(newTrajectory[i], otherTrajectory[i]);
                // 衝突は各弾の半径の和以下の場合に発生
                if(dist <= (newBulletRadius + otherRadius)) {
                    return true;
                }
            }
        }
        return false;
    }

    // 発射位置 spawnPos、ユーザー入力による初速度 initialVelocity、
    // 発射する新規弾の半径 newBulletRadius、および既存弾リスト existingBullets を基に安全な初速度を計算
    public Vector2 GetSafeVelocity(Vector2 spawnPos, Vector2 initialVelocity, float newBulletRadius, List<BulletData> existingBullets) {
        if (!enableSafeTrajectory) {
            return initialVelocity;
        }
        
        // 既存弾の軌道と半径をタプルとしてまとめる
        List<(List<Vector2> trajectory, float radius)> existingTrajectories = new List<(List<Vector2> trajectory, float radius)>();
        foreach (var b in existingBullets) {
            List<Vector2> traj = SimulateTrajectory(b.position, b.velocity);
            existingTrajectories.Add((traj, b.radius));
        }
        
        // ユーザー入力の初速度による軌道をシミュレーション
        List<Vector2> newTrajectory = SimulateTrajectory(spawnPos, initialVelocity);
        if (!WillCollide(newTrajectory, newBulletRadius, existingTrajectories)) {
            // 衝突がなければそのまま返す
            return initialVelocity;
        }
        
        // 安全な軌道が見つかるまで、初速度の向きを正負にずらして検証
        for (int attempt = 1; attempt <= maxAttempts; attempt++) {
            float angle = attempt * angleStep;
            // 正方向の調整
            Vector2 adjustedVelocity = RotateVector(initialVelocity, angle);
            List<Vector2> adjustedTrajectory = SimulateTrajectory(spawnPos, adjustedVelocity);
            if (!WillCollide(adjustedTrajectory, newBulletRadius, existingTrajectories)) {
                return adjustedVelocity;
            }
            // 負方向の調整
            adjustedVelocity = RotateVector(initialVelocity, -angle);
            adjustedTrajectory = SimulateTrajectory(spawnPos, adjustedVelocity);
            if (!WillCollide(adjustedTrajectory, newBulletRadius, existingTrajectories)) {
                return adjustedVelocity;
            }
        }
        // 試行回数内に安全な軌道が見つからなければ、元の初速度を返す
        return initialVelocity;
    }

    // 与えられたベクトル v を angleDegrees 度回転させた新たなベクトルを返す
    public Vector2 RotateVector(Vector2 v, float angleDegrees) {
        float rad = angleDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }
}
