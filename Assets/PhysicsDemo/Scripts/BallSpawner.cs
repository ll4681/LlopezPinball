using System.Collections.Generic;
using UnityEngine;

namespace PhysicsDemo
{
    // DEMO B: drops pinballs above the flippers. Press B for another.
    public class BallSpawner : MonoBehaviour
    {
        public Sprite ballSprite;
        public Material spriteMaterial;
        public PhysicsMaterial2D ballMaterial;
        public Color ballColor = Color.white;
        public float diameter = 0.5f;
        public float gravityScale = 2f;
        public int maxBalls = 5;

        public Vector2 leftRange = new Vector2(3.5f, 7f);
        public Vector2 rightRange = new Vector2(11f, 14.5f);
        public float spawnY = 8f;

        public AudioClip bounceSound;

        readonly Queue<GameObject> balls = new Queue<GameObject>();

        void Start() => Spawn();

        void Update()
        {
           if (DemoInput.Pressed(KeyCode.B)) Spawn();
        }

        public void Spawn()
        {
            Vector2 range = Random.value < 0.5f ? leftRange : rightRange;
            var go = new GameObject("Pinball");
            go.transform.SetParent(transform, false);
            go.transform.position = new Vector3(Random.Range(range.x, range.y), spawnY, 0f);
            go.transform.localScale = Vector3.one * diameter;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = ballSprite;
            sr.color = ballColor;
            if (spriteMaterial != null) sr.sharedMaterial = spriteMaterial;

            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 1.2f;
            col.sharedMaterial = ballMaterial;

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = gravityScale;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // fast + small = Continuous
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            var ballBehaviorScript = go.AddComponent<BallBehavior>();
            ballBehaviorScript.bounceSound = bounceSound;

            balls.Enqueue(go);
            while (balls.Count > maxBalls) Destroy(balls.Dequeue());
        }
    }
}
