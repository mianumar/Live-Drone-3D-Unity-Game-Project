using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    public string playerName;
    public string userId;


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