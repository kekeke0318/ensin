using UnityEngine;
using MessagePipe;
using VContainer;

public class InputService
{
    [Inject] StageData _stageData;
    [Inject] GlobalMessage _globalMessage;

    private Vector2 dragStartPos;
    private bool isDragging = false;
    GameObject previewActor;

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            previewActor = Object.Instantiate(_stageData.previewActorPrefab, dragStartPos, Quaternion.identity);
            isDragging = true;
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            // プレビューや軌道予測の更新処理（必要に応じて実装）
        }

        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            Vector2 dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 rawVector = dragEndPos - dragStartPos;
            Vector2 launchVector = SnapVector(rawVector);
            Object.Destroy(previewActor);

            _globalMessage.actorLaunchedPub.Publish(new ActorLaunchedEvent { LaunchVector = launchVector });
        }
    }

    // 角度10°、強さ0.5 単位のスナップ処理
    public Vector2 SnapVector(Vector2 raw)
    {
        float angle = Mathf.Atan2(raw.y, raw.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(angle / 10f) * 10f;
        float magnitude = raw.magnitude;
        float snappedMagnitude = Mathf.Round(magnitude / 0.5f) * 0.5f;
        Vector2 snappedVector =
            new Vector2(Mathf.Cos(snappedAngle * Mathf.Deg2Rad), Mathf.Sin(snappedAngle * Mathf.Deg2Rad)) *
            snappedMagnitude;
        return snappedVector;
    }
}