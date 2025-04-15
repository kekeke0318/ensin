using UnityEngine;
using System.Collections.Generic;

public class BulletManager : MonoBehaviour {
    [Header("物理シミュレーション設定")]
    [SerializeField] PhysicsSimulationSettings simulationSettings;

    public float bulletRadius = 0.2f; // 弾の当たり判定用半径

    [Header("Bullet プレファブ")]
    public GameObject bulletPrefab;

    // 管理中の BulletController リスト
    private List<BulletController> bulletControllers = new List<BulletController>();

    // TrajectoryAssist のインスタンス（BulletManager の設定を渡す）
    public TrajectoryAssist trajectoryAssist = new TrajectoryAssist();

    void Start() {
        // TrajectoryAssist に共通設定を適用
        trajectoryAssist.Initialize(simulationSettings);
    }

    void Update() {
        // Time.deltaTime を settings.stepTime ごとに分割してシミュレーション
        float dt = Time.deltaTime;
        int steps = Mathf.CeilToInt(dt / simulationSettings.stepTime);
        float subDt = dt / steps;
        for (int i = 0; i < steps; i++) {
            UpdateBullets(subDt);
        }
    }

    // 各弾の物理計算と衝突判定を実施し、視覚的な更新も行う
    void UpdateBullets(float dt) {
        // 各弾の位置・速度の更新
        foreach (var controller in bulletControllers) {
            if (controller == null || controller.bulletData == null) continue;
            BulletData b = controller.bulletData;
            Vector2 acceleration = -b.position * simulationSettings.gravitationalPower;
            b.velocity += acceleration * dt;
            b.position += b.velocity * dt;
        }

        // 衝突判定（各弾ペア間の距離が半径の和以下なら衝突）
        List<BulletController> collided = new List<BulletController>();
        for (int i = 0; i < bulletControllers.Count; i++) {
            for (int j = i + 1; j < bulletControllers.Count; j++) {
                if (bulletControllers[i] == null || bulletControllers[j] == null) continue;
                float dist = Vector2.Distance(bulletControllers[i].bulletData.position, bulletControllers[j].bulletData.position);
                if (dist <= bulletControllers[i].bulletData.radius + bulletControllers[j].bulletData.radius) {
                    if (!collided.Contains(bulletControllers[i])) collided.Add(bulletControllers[i]);
                    if (!collided.Contains(bulletControllers[j])) collided.Add(bulletControllers[j]);
                }
            }
        }
        // 衝突した弾はリストから削除し、GameObject を破棄
        foreach (var controller in collided) {
            if (controller != null) {
                bulletControllers.Remove(controller);
                Destroy(controller.gameObject);
            }
        }

        // 各弾の見た目の更新
        foreach (var controller in bulletControllers) {
            if (controller != null)
                controller.UpdateVisual();
        }
    }

    // 発射位置と初速度から、弾プレファブを生成する
    public void SpawnBullet(Vector2 position, Vector2 initialVelocity) {
        BulletData newBulletData = new BulletData(position, initialVelocity, bulletRadius);
        GameObject bulletGO = Instantiate(bulletPrefab, position, Quaternion.identity);
        BulletController controller = bulletGO.GetComponent<BulletController>();
        if (controller != null) {
            controller.Initialize(newBulletData);
            bulletControllers.Add(controller);
        } else {
            Debug.LogError("BulletPrefab に BulletController コンポーネントが必要です。");
        }
    }

    // 現在管理中のすべての BulletData リストを取得する（TrajectoryAssist 用）
    public List<BulletData> GetBulletDataList() {
        List<BulletData> list = new List<BulletData>();
        foreach (var controller in bulletControllers) {
            if (controller != null && controller.bulletData != null) {
                list.Add(controller.bulletData);
            }
        }
        return list;
    }

    // デバッグ用：各弾の当たり判定範囲をシーンビュー上に描画
    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        if (bulletControllers == null)
            return;
        foreach (var controller in bulletControllers) {
            if (controller != null && controller.bulletData != null)
                Gizmos.DrawWireSphere(controller.bulletData.position, controller.bulletData.radius);
        }
    }
}
