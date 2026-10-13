using UnityEngine;

public class ScoreAdjuster : MonoBehaviour
{


    public AudioClip hitSound;
    private AudioSource audioSource;



    //you can have the manager live in the script
    //public GameManager gameManager;

    public int scoreEffect;


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
        if(collision.collider.name == "Pinball")
        {
            Debug.Log("the ball has hit this target");
             //gameManager.score = gameManager.score + 1;
            GameManager.singleton.score += scoreEffect;

            if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

        }
    }


}
