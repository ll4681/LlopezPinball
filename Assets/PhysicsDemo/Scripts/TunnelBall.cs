using UnityEngine;

namespace PhysicsDemo
{
    // DEMO A: a ball bouncing between walls, with a thin wall in its path.
    // Crank the speed in Discrete mode and it tunnels through. Continuous fixes it,
    // unless you move the ball by its Transform, which is a teleport that nothing can sweep.
    [RequireComponent(typeof(Rigidbody2D))]
    public class TunnelBall : MonoBehaviour
    {
        [Header("Speed (units per second)")]
        public float speed = 20f;
        public float speedStep = 10f;
        public float minSpeed = 10f;
        public float maxSpeed = 80f;

        [Header("The thin wall")]
        public string thinWallName = "ThinWall";
        public float thinWallX = -5f;
        public float thinWallThickness = 0.1f;

        public bool TeleportMode { get; private set; }
        public int WallHits { get; private set; }
        public int Escapes { get; private set; }
        public float Diameter { get; private set; }
        public Rigidbody2D Body { get; private set; }

        Vector3 startPos;
        float teleportDir = 1f;
        bool wasBeyond;

        void Awake()
        {
            Body = GetComponent<Rigidbody2D>();
            startPos = transform.position;
            var circle = GetComponent<CircleCollider2D>();
            Diameter = circle != null ? circle.radius * 2f * transform.lossyScale.x : 0.5f;
        }

        void Update()
        {
            if (DemoInput.Pressed(KeyCode.Space)) Launch();
            if (DemoInput.Pressed(KeyCode.UpArrow)) ChangeSpeed(speedStep);
            if (DemoInput.Pressed(KeyCode.DownArrow)) ChangeSpeed(-speedStep);
            if (DemoInput.Pressed(KeyCode.C)) ToggleDetection();
            if (DemoInput.Pressed(KeyCode.T)) ToggleTeleport();
            if (DemoInput.Pressed(KeyCode.R)) ResetAll();

            if (TeleportMode)
            {
                // THE BUG: moving a physics object through its Transform.
                // Each frame the ball just appears somewhere new. No sweep, so Continuous can't help.
                transform.position += Vector3.right * teleportDir * speed * Time.deltaTime;
            }

            bool beyond = transform.position.x > thinWallX;
            if (beyond && !wasBeyond) Escapes++;
            wasBeyond = beyond;

            // Safety net if the ball ends up somewhere silly.
            if (Mathf.Abs(transform.position.x - startPos.x) > 40f) ResetPosition();
        }

        void Launch()
        {
            ResetPosition();
            teleportDir = 1f;
            if (!TeleportMode) Body.linearVelocity = Vector2.right * speed;
        }

        void ChangeSpeed(float delta)
        {
            speed = Mathf.Clamp(speed + delta, minSpeed, maxSpeed);
            if (!TeleportMode && Body.linearVelocity.sqrMagnitude > 0.01f)
                Body.linearVelocity = Body.linearVelocity.normalized * speed;
        }

        void ToggleDetection()
        {
            Body.collisionDetectionMode =
                Body.collisionDetectionMode == CollisionDetectionMode2D.Continuous
                    ? CollisionDetectionMode2D.Discrete
                    : CollisionDetectionMode2D.Continuous;
        }

        void ToggleTeleport()
        {
            TeleportMode = !TeleportMode;
            if (TeleportMode)
            {
                teleportDir = Body.linearVelocity.x < 0f ? -1f : 1f;
                Body.linearVelocity = Vector2.zero;
                // Kinematic so physics doesn't fight our teleporting; full contacts so we still hear about hits.
                Body.bodyType = RigidbodyType2D.Kinematic;
                Body.useFullKinematicContacts = true;
            }
            else
            {
                Body.bodyType = RigidbodyType2D.Dynamic;
                Body.gravityScale = 0f;
                Body.linearVelocity = Vector2.right * teleportDir * speed;
            }
        }

        void ResetPosition()
        {
            Body.linearVelocity = Vector2.zero;
            Body.position = startPos;
            transform.position = startPos;
            wasBeyond = false;
        }

        void ResetAll()
        {
            ResetPosition();
            WallHits = 0;
            Escapes = 0;
        }

        void OnCollisionEnter2D(Collision2D c)
        {
            if (c.collider.name == thinWallName) WallHits++;
            if (TeleportMode) teleportDir = -teleportDir; // fake a bounce
        }

        // How far the ball jumps between collision checks.
        public float DistancePerCheck =>
            speed * (TeleportMode ? Time.deltaTime : Time.fixedDeltaTime);

        // Rough danger line: jump farther than this and the ball can skip the thin wall.
        public float DangerDistance => Diameter + thinWallThickness;
    }
}
