using System;
using Cysharp.Threading.Tasks;
using R3;
using R3.Triggers;

namespace Project
{
    using UnityEngine;

    public class DragToShoot : MonoBehaviour
    {
        public GameObject prefab; // 生成するプレファブ
        public float forceMultiplier = 10f; // 力の倍率

        [SerializeField] ScoreTracker _scoreTracker;

        private Vector3 dragStartPos;
        private Vector3 dragEndPos;
        private bool isDragging = false;
        Camera _camera;

        void Start()
        {
            _camera = Camera.main;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // ドラッグ開始位置を記録
                dragStartPos = Input.mousePosition;
                isDragging = true;
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                // ドラッグ終了位置を記録
                dragEndPos = Input.mousePosition;
                isDragging = false;

                // ドラッグ方向と距離を計算
                Vector3 dragVector = dragStartPos - dragEndPos;
                Vector3 force = new Vector3(dragVector.x, dragVector.y, 0) * forceMultiplier;

                // プレファブを生成し、力を加える
                GameObject instance = Instantiate(prefab,
                    _camera.ScreenToWorldPoint(new Vector3(dragStartPos.x, dragStartPos.y,
                        -_camera.transform.position.z)),
                    Quaternion.identity);
                Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(force, ForceMode2D.Impulse);
                    _scoreTracker.Add(rb);

                    rb.OnCollisionEnter2DAsObservable().Subscribe(x => { _scoreTracker.Remove(rb); }).AddTo(this);
                }
            }
        }
    }
}