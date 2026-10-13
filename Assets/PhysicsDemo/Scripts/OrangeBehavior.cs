

using UnityEngine;

namespace PhysicsDemo
{
    public class OrangeBehavior : MonoBehaviour
    {
        public OrangeSpawner spawner;
        public AudioClip collectSound;
        public int points = 5;

        private bool collected = false;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<BallBehavior>() != null && !collected)
            {
                collected = true;

                
                GameManager.singleton.score += points;

                
                if (collectSound != null && Camera.main != null)
                {
                    AudioSource.PlayClipAtPoint(collectSound,Camera.main.transform.position);
                }

                
                spawner.RespawnOrange();

                
                Destroy(gameObject);
            }
        }
    }
}