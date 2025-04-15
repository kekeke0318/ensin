using System;
using UnityEngine;

public class Mover : MonoBehaviour
{
    Transform _t;
    
    void Awake()
    {
        _t = transform;
    }

    public void AddForce(Vector2 vector)
    {
        _t.Translate(vector);
    }
}