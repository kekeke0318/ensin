using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] Camera _cam;

    public Vector3 ScreenToWorldPoint(Vector3 position)
    {
        return _cam.ScreenToWorldPoint(position);
    }
}
