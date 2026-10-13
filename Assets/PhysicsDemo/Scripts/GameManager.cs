using TMPro;
using UnityEngine;
using PhysicsDemo;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    //static means this is the one true copy of the object
    //singleton game manager object; means we can access this reference from anywhere, and it will give you access to GameManager
    public static GameManager singleton;

    public TextMeshProUGUI scoreValueLabel;
    public TextMeshProUGUI livesValueLabel;

    public int score = 0;
    public int lives = 3;

  //  bool isGameOver = false;
    public bool isGameOver = false;

    public BallSpawner ballSpawner;
 //spawner is in the other Unity project


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        scoreValueLabel.text = score.ToString();

        if (lives >= 0)
        {
            livesValueLabel.text = lives.ToString();
        } else
        {
            livesValueLabel.text = "0";
        }

        if (lives <= 0)
        {

            isGameOver = true;

            if (DemoInput.Pressed(KeyCode.R))
            {
                RestartGame();
            }

        }

    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
