using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using AL.Monetization.CustomAdmobAds.Scripts;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public Text scoreText; 
    public Slider scoreSlider; 
    public Slider healthSlider; 
    public GameObject levelCompletePanel; 
    public GameObject deadPanel; 
    public Image[] hearts; // Array to hold the heart images
    public Text livesPopupText; // Popup text for remaining lives
    public DroneMovement droneMovement; // Reference to the DroneMovement script
    public DronePropelers DronePropelers; // Reference to the DronePropelers script
    private int score = 0;
    public int maxScore = 100; 
    public float playerHealth = 100f; 
    public float maxHealth = 100f; 
    public float healthDecreaseRate = 5f; 
    private int lives = 3; // Number of hearts/lives
    private bool isHealthDecreasing = true; // Flag to control health decrease
    private bool isAdShown = false; // Flag to control Ad

    private void Awake()
    {
        isAdShown = false ;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        isAdShown = false;
        Time.timeScale = 1;
        UpdateScoreText();
        UpdateScoreSlider();
        UpdateHealthSlider();
        levelCompletePanel.SetActive(false); 
        deadPanel.SetActive(false); 
        livesPopupText.gameObject.SetActive(false); // Hide popup text at the start
        UpdateHearts(); // Ensure the hearts are correctly displayed at the start
    }

    private void Update()
    {
        if (isHealthDecreasing)
        {
            DecreaseHealthOverTime();
        }
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreText();
        UpdateScoreSlider();

        if (score >= maxScore)
        {
            LevelComplete();
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Gold Score: " + score.ToString();
    }

    private void UpdateScoreSlider()
    {
        scoreSlider.value = (float)score / maxScore;
    }

    private void DecreaseHealthOverTime()
    {
        playerHealth -= healthDecreaseRate * Time.deltaTime;
        playerHealth = Mathf.Clamp(playerHealth, 0, maxHealth);
        UpdateHealthSlider();

        if (playerHealth <= 0)
        {
            LoseLife();
        }
    }

    public void IncreaseHealth(float healthValue)
    {
        playerHealth += healthValue;
        playerHealth = Mathf.Clamp(playerHealth, 0, maxHealth);
        UpdateHealthSlider();
    }

    private void UpdateHealthSlider()
    {
        healthSlider.value = playerHealth / maxHealth;
    }

    private void LoseLife()
    {
        lives--;
        UpdateHearts();

        if (lives > 0)
        {
            StartCoroutine(HandleDroneDisable()); // Start the coroutine to disable and re-enable the drone
            playerHealth = maxHealth; // Reset health for the next life
            UpdateHealthSlider();
        }
        else
        {
            PlayerDead();
        }
    }

    private IEnumerator HandleDroneDisable()
    {
        isHealthDecreasing = false; // Stop health decrease
        droneMovement.enabled = false; // Disable the DroneMovement script
        DronePropelers.enabled = false; // Disable the DronePropelers script
        livesPopupText.text = "Lives Remaining: " + lives;
        livesPopupText.gameObject.SetActive(true); // Show the popup text
        yield return new WaitForSeconds(5f); // Wait for 3 seconds
        livesPopupText.gameObject.SetActive(false); // Hide the popup text
        droneMovement.enabled = true; // Re-enable the DroneMovement script
        DronePropelers.enabled = true; // Re-enable the DronePropelers script
        isHealthDecreasing = true; // Resume health decrease
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < lives; // Enable or disable heart images based on the remaining lives
        }
    }

    private void LevelComplete()
    {
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.MarkCurrentSceneLevelComplete();
        }
        levelCompletePanel.SetActive(true);
        Time.timeScale = 0;
        loadInterAd();
    }

    private void PlayerDead()
    {
        deadPanel.SetActive(true);
        Time.timeScale = 0;
        Debug.Log("Player is dead");
        loadInterAd();
    }

    public void loadInterAd()
    {
        if (!isAdShown)
        {
            AdManager.instance.ShowInterstitial();
            isAdShown = true;
        }
    }
    public void level1()
    {
        SceneManager.LoadScene("Level_1");
    }
    public void level2()
    {
        SceneManager.LoadScene("Level_2");
    }
    public void level3()
    {
        SceneManager.LoadScene("Level_3");
    }
    public void level4()
    {
        SceneManager.LoadScene("Level_4");
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("Main_Menu");
    }
     public void level5()
    {
        SceneManager.LoadScene("Level_5");
    }
     public void level6()
    {
        SceneManager.LoadScene("Level_6");
    }
     public void level7()
    {
        SceneManager.LoadScene("Level_7");
    }
     public void level8()
    {
        SceneManager.LoadScene("Level_8");
    }
    public void level9()
    {
        SceneManager.LoadScene("Level_9");
    }
    public void level10()
    {
        SceneManager.LoadScene("Level_10");
    }

    public void level11()
    {
        SceneManager.LoadScene("Level_11");
    }

    public void level12()
    {
        SceneManager.LoadScene("Level_12");
    }

    public void level13()
    {
        SceneManager.LoadScene("Level_13");
    }

    public void level14()
    {
        SceneManager.LoadScene("Level_14");
    }

    public void level15()
    {
        SceneManager.LoadScene("Level_15");
    }

    public void level16()
    {
        SceneManager.LoadScene("Level_16");
    }

}
