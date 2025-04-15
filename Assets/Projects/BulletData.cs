using UnityEngine;

public class BulletData {
    public Vector2 position;
    public Vector2 velocity;
    public float radius;

    public BulletData(Vector2 position, Vector2 velocity, float radius) {
        this.position = position;
        this.velocity = velocity;
        this.radius = radius;
    }
}