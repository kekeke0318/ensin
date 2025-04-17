using UnityEngine;
using MessagePipe;
using VContainer;

/// <summary>
/// ドラッグ入力 → 発射ベクトル算出 → 軌道プレビュー描画 → 発射イベント送信
/// </summary>
public class InputService
{
    StageData _stageData;
    [Inject] MainCamera _mainCamera;
    [Inject] GlobalMessage _globalMessage;
    [Inject] TrajectoryLine _trajectoryLine; // Scene 上の LineRenderer を DI

    Vector2 dragStartPos;
    bool isDragging;
    GameObject previewActor;
    TrajectoryAssist _assist;

    public InputService(StageData stageData)
    {
        _stageData = stageData;
        _assist = new TrajectoryAssist(_stageData.gravity);
    }
    
    public void Update()
    {
        // ドラッグ開始
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            previewActor = Object.Instantiate(
                _stageData.previewActorPrefab,
                dragStartPos,
                Quaternion.identity);

            isDragging = true;
        }

        // ドラッグ中 ― 軌道を予測描画
        if (isDragging && Input.GetMouseButton(0))
        {
            Vector2 dragNow = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 rawVector = dragNow - dragStartPos;
            EnsinLog.Info(rawVector);
            Vector2 launchVector = SnapVector(rawVector);

            Vector2[] points = _assist.SimulateTrajectory(dragStartPos, launchVector);
            _trajectoryLine.SetPositionCount(points.Length);
            for (int i = 0; i < points.Length; i++)
                _trajectoryLine.SetPosition(i, points[i]);
        }

        // ドラッグ終了 ― 発射
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;

            Vector2 dragEndPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 rawVector = dragEndPos - dragStartPos;
            
            Vector2 launchVector = SnapVector(rawVector);
            EnsinLog.Info(rawVector);
            EnsinLog.Info(launchVector);

            Object.Destroy(previewActor);
            if (_trajectoryLine != null) _trajectoryLine.SetPositionCount(0);

            _globalMessage.actorLaunchedPub
                .Publish(new ActorLaunchedEvent { LaunchVector = launchVector });
        }
    }

    // 角度 10°・強さ 0.5 単位でスナップ
    Vector2 SnapVector(Vector2 raw)
    {
        float angle = Mathf.Atan2(raw.y, raw.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(angle / 10f) * 10f;
        float magnitude = raw.magnitude;
        float snappedMag = Mathf.Round(magnitude / 0.5f) * 0.5f;
        return new Vector2(
            Mathf.Cos(snappedAngle * Mathf.Deg2Rad),
            Mathf.Sin(snappedAngle * Mathf.Deg2Rad)) * snappedMag;
    }
}