using UnityEngine;

namespace PhysicsDemo
{
    // THE FIX: a Kinematic Rigidbody2D rotated with MoveRotation.
    // Physics knows the paddle is moving and how fast, so it actually hits the ball.
    // (A HingeJoint2D with a motor is the other common approach.)
    [RequireComponent(typeof(Rigidbody2D))]
    public class PhysicsFlipper : MonoBehaviour
    {
        public KeyCode key = KeyCode.LeftArrow;
        public float restAngle = -25f;
        public float activeAngle = 25f;
        [Tooltip("Degrees per second")] public float speed = 1000f;

        Rigidbody2D rb;
        bool held;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Read input in Update, move physics objects in FixedUpdate.
        void Update() => held = DemoInput.Held(key);

        void FixedUpdate()
        {
            float target = held ? activeAngle : restAngle;
            float next = Mathf.MoveTowardsAngle(rb.rotation, target, speed * Time.fixedDeltaTime);
            rb.MoveRotation(next);
        }
    }
}
