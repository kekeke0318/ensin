namespace AIBirthGame
{
using UnityEngine;

public class InputController : MonoBehaviour
{
    public GameObject actorPrefab;
    private GameObject previewActor;
    private Vector2 dragStartPos;
    private bool isDragging = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            previewActor = Instantiate(actorPrefab, dragStartPos, Quaternion.identity);
            isDragging = true;
        }
        if (isDragging && Input.GetMouseButton(0))
        {
            // ここでTrajectoryAssistを呼び出し、プレビューActorの位置や軌道予測を更新する処理を実装
        }
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            Vector2 dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 rawVector = dragEndPos - dragStartPos;
            Vector2 launchVector = SnapVector(rawVector);
            // 発射処理を実施（例：Rigidbodyに力を加える等）
            Destroy(previewActor);
            // MessagePipeを使い、「発射完了」イベントを発行する例も追加可能
        }
    }

    // スナップ処理（角度10°、強さ0.5単位）
    public Vector2 SnapVector(Vector2 raw)
    {
        float angle = Mathf.Atan2(raw.y, raw.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(angle / 10f) * 10f;
        float magnitude = raw.magnitude;
        float snappedMagnitude = Mathf.Round(magnitude / 0.5f) * 0.5f;
        Vector2 snappedVector = new Vector2(Mathf.Cos(snappedAngle * Mathf.Deg2Rad), Mathf.Sin(snappedAngle * Mathf.Deg2Rad)) * snappedMagnitude;
        return snappedVector;
    }
}

}