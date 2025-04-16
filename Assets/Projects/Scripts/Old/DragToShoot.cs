using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class DragToShoot : MonoBehaviour
{
    public BulletManager bulletManager;
    public float forceMultiplier = 10f;
    public TrajectoryAssist trajectoryAssist;

    private Vector3 dragStartPos;
    private Vector3 dragEndPos;
    private bool isDragging = false;
    private Camera _camera;

    private BulletController previewBullet; // ドラッグ中に表示する弾

    void Start()
    {
        _camera = Camera.main;
        if (trajectoryAssist == null && bulletManager != null)
        {
            trajectoryAssist = bulletManager.trajectoryAssist;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Input.mousePosition;
            isDragging = true;

            // ドラッグ開始時に仮の弾を生成（静止状態）
            Vector3 worldPos =
                _camera.ScreenToWorldPoint(new Vector3(dragStartPos.x, dragStartPos.y, -_camera.transform.position.z));
            Vector2 spawnPos = new Vector2(worldPos.x, worldPos.y);
            previewBullet = bulletManager.SpawnPreviewBullet(spawnPos); // <- ここ新設
        }
        
        // ⬇ ドラッグ中に弾を引っ張るように位置更新
        if (isDragging && previewBullet != null) {
            Vector3 currentPos = Input.mousePosition;
            Vector3 dragVector = (currentPos - dragStartPos);
            Vector3 worldStart = _camera.ScreenToWorldPoint(new Vector3(dragStartPos.x, dragStartPos.y, -_camera.transform.position.z));
            Vector3 worldOffset = _camera.ScreenToWorldPoint(new Vector3(currentPos.x, currentPos.y, -_camera.transform.position.z)) - worldStart;
        
            // 弾の座標更新（逆方向に引っ張る）
            Vector2 previewPos = (Vector2)worldStart + (Vector2)worldOffset * 0.5f;
            previewBullet.bulletData.position = previewPos;
            previewBullet.UpdateVisual(); // 表示位置も更新
        }
        
        if (Input.GetMouseButtonUp(0) && isDragging) {
            dragEndPos = Input.mousePosition;
            isDragging = false;

            Vector3 dragVector = dragStartPos - dragEndPos;
            if (dragVector.sqrMagnitude < 0.1f) return;

            Vector2 userInitialVelocity = new Vector2(dragVector.x, dragVector.y) * forceMultiplier;

            if (previewBullet != null) {
                Vector2 actualSpawnPos = previewBullet.bulletData.position; // ← 実際の現在位置
                float newBulletRadius = bulletManager.bulletRadius;
                List<BulletData> existingBullets = bulletManager.GetBulletDataList();

                Vector2 safeVelocity = trajectoryAssist.GetSafeVelocity(actualSpawnPos, userInitialVelocity, newBulletRadius, existingBullets);

                previewBullet.bulletData.velocity = safeVelocity;
                bulletManager.RegisterBullet(previewBullet);
                previewBullet = null;
            }
        }

    }
}