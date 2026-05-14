using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    public string playerName;
    public string userId;
    public int totalScore;

    [Space(30)]
    public bool TempBonusLevelTestOn = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        playerName = PlayerPrefs.GetString("PLAYER_NAME", "");
        userId = PlayerPrefs.GetString("USER_ID", "");

        // Load the total score from previous sessions
        totalScore = PlayerPrefs.GetInt("TOTAL_SCORE", 0);

        if (string.IsNullOrEmpty(userId))
        {
            userId = SystemInfo.deviceUniqueIdentifier;
            PlayerPrefs.SetString("USER_ID", userId);
        }

        if (TempBonusLevelTestOn) {
            if (!PlayerPrefs.HasKey("UnlockedLevel"))
            {
                PlayerPrefs.SetInt("UnlockedLevel", 8);
                PlayerPrefs.Save();
            }
        }
    }

    public void AddToTotalScore(int levelScore, int livesLeft)
    {
        // 🔥 Multiplier Logic: 
        // 3 lives = 2.0x bonus
        // 2 lives = 1.5x bonus
        // 1 life  = 1.0x (no bonus)
        float multiplier = 1f;
        if (livesLeft == 3) multiplier = 2.0f;
        else if (livesLeft == 2) multiplier = 1.5f;

        int finalLevelScore = Mathf.RoundToInt(levelScore * multiplier);
        totalScore += finalLevelScore;

        // Save to local storage
        PlayerPrefs.SetInt("TOTAL_SCORE", totalScore);
        PlayerPrefs.Save();

        // Sync with Web Database
        if (SQLHighScoreManager.Instance != null)
        {
            SQLHighScoreManager.Instance.UpdateHighScore(totalScore);
        }
    }


    public bool HasName()
    {
        return !string.IsNullOrEmpty(playerName);
    }

    public void SaveName(string name)
    {
        playerName = name;
        PlayerPrefs.SetString("PLAYER_NAME", playerName);
        PlayerPrefs.Save();
    }
}