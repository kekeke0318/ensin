using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] Camera _cam;

    public Vector3 ScreenToWorldPoint(Vector3 position)
    {
        Vector3 pos = position;
        pos.z = -_cam.transform.position.z;
        return _cam.ScreenToWorldPoint(pos);
    }
}
