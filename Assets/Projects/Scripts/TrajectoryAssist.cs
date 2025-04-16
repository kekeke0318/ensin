using UnityEngine;

public class TrajectoryAssist
{
    // ステップごとの時間間隔（秒）
    private readonly float timeStep = 0.1f;
    // 軌道予測の試行回数（これで軌道の長さを決める）
    private readonly int maxSteps = 30;
    private readonly float gravity;

    // StageData から取得した重力値を利用して初期化
    public TrajectoryAssist(float gravity)
    {
        this.gravity = gravity;
    }

    // 指定された初期位置・発射ベクトルでシミュレーションし、軌道上のポイントを返す
    public Vector2[] SimulateTrajectory(Vector2 startPosition, Vector2 initialVelocity)
    {
        Vector2[] positions = new Vector2[maxSteps];
        Vector2 pos = startPosition;
        Vector2 vel = initialVelocity;
        for (int i = 0; i < maxSteps; i++)
        {
            pos += vel * timeStep;
            // 重力を反映（Y方向のみ）
            vel.y += gravity * timeStep;
            positions[i] = pos;
        }
        return positions;
    }
}
