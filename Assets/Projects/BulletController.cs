using UnityEngine;

public class BulletController : MonoBehaviour {
    public BulletData bulletData;

    // BulletData を初期化する
    public void Initialize(BulletData data) {
        bulletData = data;
        transform.position = data.position;
    }

    // BulletData の更新結果を反映して表示位置を更新
    public void UpdateVisual() {
        transform.position = bulletData.position;
    }
}