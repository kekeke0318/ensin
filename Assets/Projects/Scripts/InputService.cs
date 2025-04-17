using UnityEngine;
using MessagePipe;
using VContainer;

/// <summary>
/// ドラッグ入力 → 発射ベクトル算出 → 軌道プレビュー描画 → 発射イベント送信
/// </summary>
public class InputService
{
    [Inject] StageData _stageData;
    [Inject] GlobalMessage _globalMessage;
    [Inject] LineRenderer _trajectoryLine;   // Scene 上の LineRenderer を DI

    Vector2 dragStartPos;
    bool isDragging;
    GameObject previewActor;
    TrajectoryAssist _assist;

    public void Update()
    {
        // StageData が注入されたタイミングで TrajectoryAssist を初期化
        if (_assist == null && _stageData != null)
            _assist = new TrajectoryAssist(_stageData.gravity);

        // ドラッグ開始
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            previewActor = Object.Instantiate(
                _stageData.previewActorPrefab,
                dragStartPos,
                Quaternion.identity);

            isDragging = true;
        }

        // ドラッグ中 ― 軌道を予測描画
        if (isDragging && Input.GetMouseButton(0))
        {
            Vector2 dragNow     = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 rawVector   = dragNow - dragStartPos;
            Vector2 launchVector = SnapVector(rawVector);

            if (_trajectoryLine != null)
            {
                Vector2[] points = _assist.SimulateTrajectory(dragStartPos, launchVector);
                _trajectoryLine.positionCount = points.Length;
                for (int i = 0; i < points.Length; i++)
                    _trajectoryLine.SetPosition(i, points[i]);
            }
        }

        // ドラッグ終了 ― 発射
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;

            Vector2 dragEndPos   = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 rawVector    = dragEndPos - dragStartPos;
            Vector2 launchVector = SnapVector(rawVector);

            Object.Destroy(previewActor);
            if (_trajectoryLine != null) _trajectoryLine.positionCount = 0;

            _globalMessage.actorLaunchedPub
                .Publish(new ActorLaunchedEvent { LaunchVector = launchVector });
        }
    }

    // 角度 10°・強さ 0.5 単位でスナップ
    Vector2 SnapVector(Vector2 raw)
    {
        float angle          = Mathf.Atan2(raw.y, raw.x) * Mathf.Rad2Deg;
        float snappedAngle   = Mathf.Round(angle / 10f) * 10f;
        float magnitude      = raw.magnitude;
        float snappedMag     = Mathf.Round(magnitude / 0.5f) * 0.5f;
        return new Vector2(
            Mathf.Cos(snappedAngle * Mathf.Deg2Rad),
            Mathf.Sin(snappedAngle * Mathf.Deg2Rad)) * snappedMag;
    }
}
