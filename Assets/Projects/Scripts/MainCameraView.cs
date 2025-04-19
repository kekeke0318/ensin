using UnityEngine;
using VContainer;

/// <summary>
/// Controls the main camera so that it moves smoothly after the latest launched <see cref="ActorView"/>.
/// </summary>
[RequireComponent(typeof(Camera))]
public class MainCameraView : MonoBehaviour
{
    public Camera Cam => _cam;

    [SerializeField] Camera _cam;
    [SerializeField] bool _useFollow;

    [Header("Follow settings")]
    [SerializeField, Tooltip("Time (in seconds) the camera needs to catch‑up. Higher = slower movement.")]
    float _smoothTime = 0.5f;

    Vector3 _velocity; // used by SmoothDamp
    ICameraTarget _target;

    // ActorManager is injected from VContainer so we know which Actor to follow
    [Inject] ActorManager _actorManager;

    void Awake()
    {
        // Fallback if the camera reference wasn’t set in the Inspector
        if (_cam == null) _cam = GetComponent<Camera>();
        
        
    }

    void LateUpdate()
    {
        if (_target == null) return;

        if (_useFollow)
        {
            // Preserve the current Z so we don’t change the projection distance
            Vector3 desired = new(_target.Transform.position.x, _target.Transform.position.y, _cam.transform.position.z);

            // Smoothly move the camera toward the desired position
            _cam.transform.position = Vector3.SmoothDamp(_cam.transform.position, desired, ref _velocity, _smoothTime);
        }
    }

    public void SetTarget(ICameraTarget t)
    {
        _target = t;
    }

    /// <summary>
    /// Utility wrapper so other systems can convert from screen to world coordinates using this camera.
    /// </summary>
    public Vector3 ScreenToWorldPoint(Vector3 position)
    {
        position.z = _cam.transform.position.z;
        return _cam.ScreenToWorldPoint(position);
    }
}
