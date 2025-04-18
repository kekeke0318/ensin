using UnityEngine;
using MessagePipe;
using R3;
using VContainer;

/// <summary>
/// ドラッグ入力 → 発射ベクトル算出 → 軌道プレビュー描画 → 発射イベント送信
/// </summary>
public class InputService
{
    StageData _stageData;
    [Inject] MotherView _motherView;
    [Inject] MainCameraView _mainCameraView;
    [Inject] GlobalMessage _globalMessage;
    [Inject] TrajectoryLineView _trajectoryLineView; // Scene 上の LineRenderer を DI

    Vector2 dragStartPos;
    bool isDragging;
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
            dragStartPos = _motherView.Position;
            isDragging = true;
        }

        // ドラッグ中 ― 軌道を予測描画
        if (isDragging && Input.GetMouseButton(0))
        {
            Vector2 dragNow = _mainCameraView.ScreenToWorldPoint(Input.mousePosition);
            Vector2 rawVector = dragNow - dragStartPos;
            Vector2 launchVector = SnapVector(rawVector);

            _trajectoryLineView.SetPositionCount(2);
            _trajectoryLineView.SetPosition(0, dragStartPos);
            _trajectoryLineView.SetPosition(1, dragNow);
        }

        // ドラッグ終了 ― 発射
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;

            Vector2 dragEndPos = _mainCameraView.ScreenToWorldPoint(Input.mousePosition);
            Vector2 rawVector = dragEndPos - dragStartPos;
            Vector2 launchVector = SnapVector(rawVector);
            
            if (_trajectoryLineView != null) _trajectoryLineView.SetPositionCount(0);

            _globalMessage.actorLaunchedPub
                .Publish(new ActorLaunchedEvent { Position =  dragStartPos, LaunchVector = launchVector });
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

    System.IDisposable _dispo;
    
    public void SetEnabled(bool p0)
    {
        if (p0)
        {
            _dispo = Observable.EveryUpdate().Subscribe(x => { Update(); });
        }
        else
        {
            _dispo?.Dispose();
        }
    }
}