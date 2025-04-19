using System;
using TMPro;
using UnityEngine;

public class ActorMessageView : MonoBehaviour
{
    [SerializeField] TMP_Text _messageText;
    [SerializeField] Vector3 _offset3d;

    Transform _target;
    Camera _cam;

    public void SetTarget(Transform t)
    {
        _target = t;
    }
    
    public void SetCamera(Camera cam)
    {
        _cam = cam;
    }

    public void SetText(string message)
    {
        _messageText.SetText(message);   
    }

    void Update()
    {
        if (_target == null) return;
        
        _target.position = _cam.WorldToScreenPoint(transform.position + _offset3d);
    }
}
