using UnityEngine;

namespace PhysicsDemo
{
    // THE BUG: no Rigidbody2D, and the paddle is rotated by setting transform.rotation.
    // Physics sees a "static" wall that keeps teleporting into the ball. It gets shoved
    // out of the overlap instead of being hit, so it dribbles, sticks, or passes through.
    public class TransformFlipper : MonoBehaviour
    {
        public KeyCode key = KeyCode.RightArrow;
        public float restAngle = 25f;
        public float activeAngle = -25f;
        [Tooltip("Degrees per second")] public float speed = 1000f;

        float angle;

        void Start()
        {
            angle = restAngle;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        void Update()
        {
            float target = DemoInput.Held(key) ? activeAngle : restAngle;
            angle = Mathf.MoveTowardsAngle(angle, target, speed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
