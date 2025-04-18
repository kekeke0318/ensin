using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using MessagePipe;
using R3;
using VContainer;

/// <summary>
/// ドラッグ入力 → 発射ベクトル算出 → 軌道プレビュー描画 → 発射イベント送信
/// </summary>
public class InputController : MonoBehaviour
{
    public Subject<ActorLaunchedEvent> OnShootInput { get; private set; } = new Subject<ActorLaunchedEvent>();
    public Subject<RetryEvent> OnRetryInput { get; private set; } = new Subject<RetryEvent>();

    [SerializeField] TrajectoryLineView _trajectoryLineView;

    Vector2 _dragStartPos;
    bool isDragging;
    System.IDisposable _dispo;
    Vector2 _worldMousePosition;
    Camera _cam;

    public void SetTrajectoryLine(TrajectoryLineView trajectoryLineView)
    {
    }

    public void SetDragStartPos(Vector3 pos)
    {
        _dragStartPos = pos;
    }
    
    public void SetCamera(Camera cam)
    {
        _cam = cam;
        EnsinLog.Info($"SetCamera {_cam}");
    }

    void Start()
    {
        EnsinLog.Info("Start");
    }

    public void Update()
    {
        _worldMousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
        
        // ドラッグ開始
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
        }

        // ドラッグ中 ― 軌道を描画
        if (isDragging && Input.GetMouseButton(0))
        {
            Vector2 rawVector = _worldMousePosition - _dragStartPos;
            Vector2 launchVector = SnapVector(rawVector);

            _trajectoryLineView.SetPositionCount(2);
            _trajectoryLineView.SetPosition(0, _dragStartPos);
            _trajectoryLineView.SetPosition(1, _worldMousePosition);
        }

        // ドラッグ終了 ― 発射
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;

            Vector2 rawVector = _worldMousePosition - _dragStartPos;
            Vector2 launchVector = SnapVector(rawVector);

            _trajectoryLineView.SetPositionCount(0);

            OnShootInput.OnNext(new ActorLaunchedEvent { Position = _dragStartPos, LaunchVector = launchVector });
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            OnRetryInput.OnNext(new RetryEvent() { });
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