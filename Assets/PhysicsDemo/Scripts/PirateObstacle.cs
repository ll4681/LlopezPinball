using UnityEngine;

namespace PhysicsDemo
{
    public class PirateObstacle : MonoBehaviour
    {
        public enum Effect { Treasure, Cannon, Tar }

        public Effect effect;
        public AudioClip hitSound;
        public float cannonForce = 10f;

        private AudioSource audioSource;
        private float lastHitTime = -10f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }


        // Update is called once per frame
        void Update()
        {

        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<BallBehavior>() == null)
                return;

            if (Time.time - lastHitTime < 0.3f)
                return;

            lastHitTime = Time.time;

            Rigidbody2D rb = collision.rigidbody;

            if (rb == null) return;

            switch (effect)
            {
                case Effect.Treasure:
                    GameManager.singleton.score += 50;
                    break;

                case Effect.Cannon:
                    rb.AddForce(Vector2.up * cannonForce,ForceMode2D.Impulse);
                    break;

                case Effect.Tar:
                    rb.linearVelocity *= 0.5f;
                    break;
            }

            if (hitSound != null)
                audioSource.PlayOneShot(hitSound);
        }
    }
}