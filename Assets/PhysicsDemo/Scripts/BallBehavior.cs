using UnityEngine;

public class BallBehavior : MonoBehaviour
{

    public bool isAlive = true;

    public AudioClip bounceSound;
    public float minBounceSpeed = 0.5f;
    public float bounceCooldown = 0.15f;

    private AudioSource audioSource;
    private float lastBounceTime = -999f;


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

    private void OnCollisionEnter2D(Collision2D collision)
    {


        if (collision.relativeVelocity.magnitude >= minBounceSpeed && Time.time - lastBounceTime >= bounceCooldown)
        {
            audioSource.PlayOneShot(bounceSound);
            lastBounceTime = Time.time;
        }


        if (collision != null)
        {
            if(collision.gameObject.name == "DrainTrigger")
            {

                if (isAlive)
                {
                    GameManager.singleton.lives--;

                    isAlive = false;

                    if(GameManager.singleton.isGameOver == false)
                    {
                        GameManager.singleton.ballSpawner.Spawn();
                    }
                }


                
            }
        }
    }

}
