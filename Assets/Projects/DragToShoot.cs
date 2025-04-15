using UnityEngine;
using System.Collections.Generic;

public class DragToShoot : MonoBehaviour {
    public BulletManager bulletManager; // BulletManager への参照
    public float forceMultiplier = 10f;

    // TrajectoryAssist のインスタンス（BulletManager 経由で共通設定済み）
    public TrajectoryAssist trajectoryAssist;

    private Vector3 dragStartPos;
    private Vector3 dragEndPos;
    private bool isDragging = false;
    Camera _camera;

    void Start() {
        _camera = Camera.main;
        // BulletManager から TrajectoryAssist を取得するか、Inspector でアサインする
        if (trajectoryAssist == null && bulletManager != null) {
            trajectoryAssist = bulletManager.trajectoryAssist;
        }
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            dragStartPos = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0) && isDragging) {
            dragEndPos = Input.mousePosition;
            isDragging = false;

            Vector3 dragVector = dragStartPos - dragEndPos;
            
            if (dragVector.sqrMagnitude < 0.1f) return;
            
            Vector2 userInitialVelocity = new Vector2(dragVector.x, dragVector.y) * forceMultiplier;
            // 画面座標をワールド座標に変換（カメラの Z 座標を反映）
            Vector3 worldPos = _camera.ScreenToWorldPoint(new Vector3(dragStartPos.x, dragStartPos.y, -_camera.transform.position.z));
            Vector2 spawnPos = new Vector2(worldPos.x, worldPos.y);

            // BulletManager で管理中の弾のデータを取得
            List<BulletData> existingBullets = bulletManager.GetBulletDataList();
            // 発射する弾の半径は bulletManager.bulletRadius を使用
            float newBulletRadius = bulletManager.bulletRadius;

            // TrajectoryAssist で安全な初速度を計算
            Vector2 safeVelocity = trajectoryAssist.GetSafeVelocity(spawnPos, userInitialVelocity, newBulletRadius, existingBullets);

            // 補正後の初速度で弾を発射
            bulletManager.SpawnBullet(spawnPos, safeVelocity);
        }
    }
}