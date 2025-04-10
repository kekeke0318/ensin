using System;

namespace Project
{
    using UnityEngine;

    public class Bullet : MonoBehaviour
    {
        [SerializeField] Rigidbody2D _rb;
        [SerializeField] float _power = 1;

        void FixedUpdate()
        {
            _rb.AddForce(-_rb.position * _power);
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            Destroy(gameObject);
        }
    }
}