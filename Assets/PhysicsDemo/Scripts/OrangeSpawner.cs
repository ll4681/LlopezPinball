
using UnityEngine;

namespace PhysicsDemo
{
    public class OrangeSpawner : MonoBehaviour
    {
        public GameObject orangePrefab;

        public Vector2 minPosition = new Vector2(3.5f, 1f);
        public Vector2 maxPosition = new Vector2(14.5f, 7f);

        public int orangeCount = 3;
        public float respawnDelay = 2f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            for (int i = 0; i < orangeCount; i++)
            {
                Spawn();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Spawn()
        {
            Vector3 position = new Vector3(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y), 0f);

            GameObject orange = Instantiate(orangePrefab,position,Quaternion.identity);

            orange.GetComponent<OrangeBehavior>().spawner = this;
        }

        public void RespawnOrange()
        {
            Invoke(nameof(Spawn), respawnDelay);
        }
    }
}