using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class ProgressManager : MonoBehaviour
{
    private const string UnlockedLevelKey = "UnlockedLevel";

    // Index 1-8: Level_1 to Level_8
    // Index 9: BonusLevel
    // Index 10-17: Level_9 to Level_16
    // Keep these in sync with your actual scenes.
    public const int MinLevelIndex = 1;
    public const int MaxLevelIndex = 17;
    public const int BonusLevelIndex = 9;

    public static ProgressManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureInstance()
    {
        if (Instance != null) return;

        var go = new GameObject(nameof(ProgressManager));
        DontDestroyOnLoad(go);
        Instance = go.AddComponent<ProgressManager>();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public int GetUnlockedLevel()
    {
        var unlocked = PlayerPrefs.GetInt(UnlockedLevelKey, MinLevelIndex);
        if (unlocked == 9)
        {
            Debug.Log("Level Bonus Need to be called here");

        }
        return Mathf.Clamp(unlocked, MinLevelIndex, MaxLevelIndex);
    }

    public bool IsLevelUnlocked(int levelIndex) => levelIndex <= GetUnlockedLevel();

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(UnlockedLevelKey);
        PlayerPrefs.Save();
    }

    public void MarkCurrentSceneLevelComplete()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Level Completed == " +  sceneName);
        int completedIndex;

        // Special check for BonusLevel
        if (sceneName.Equals("BonusLevel", StringComparison.OrdinalIgnoreCase))
        {
            completedIndex = BonusLevelIndex;
        }
        else if (!TryParseLevelIndex(sceneName, out completedIndex))
        {
            return;
        }

        //MarkLevelComplete(SceneManager.GetActiveScene().name);
        MarkLevelCompleteByIndex(completedIndex);
    }

    public void MarkLevelComplete(string sceneName)
    {
        if (!TryParseLevelIndex(sceneName, out var completedLevel)) return;

        // Completing level N unlocks level N+1 (capped).
        var nextToUnlock = Mathf.Clamp(completedLevel + 1, MinLevelIndex, MaxLevelIndex);
        var currentUnlocked = GetUnlockedLevel();

        if (nextToUnlock <= currentUnlocked) return;

        PlayerPrefs.SetInt(UnlockedLevelKey, nextToUnlock);
        PlayerPrefs.Save();
        Debug.Log($"Progress saved. Unlocked level is now: {nextToUnlock}");
    }

    private void MarkLevelCompleteByIndex(int completedLevel)
    {
        var nextToUnlock = Mathf.Clamp(completedLevel + 1, MinLevelIndex, MaxLevelIndex);
        var currentUnlocked = GetUnlockedLevel();

        if (nextToUnlock <= currentUnlocked) return;

        PlayerPrefs.SetInt(UnlockedLevelKey, nextToUnlock);
        PlayerPrefs.Save();
        Debug.Log($"Progress saved. Unlocked index is now: {nextToUnlock}");
    }

    public string GetContinueSceneName()
    {
        int unlocked = GetUnlockedLevel();

        // LOGIC FOR SCENE NAMING
        if (unlocked < BonusLevelIndex)
        {
            return $"Level_{unlocked}";
        }
        else if (unlocked == BonusLevelIndex)
        {
            return "BonusLevel";
        }
        else
        {
            // For indices 10-17, we need to load Level_9 through Level_16
            // Calculation: index - 1 (e.g., Index 10 loads Level_9)
            return $"Level_{unlocked - 1}";
        }
        // Continue at the highest unlocked level.
    }

    private static bool TryParseLevelIndex(string sceneName, out int levelIndex)
    {
        levelIndex = 0;
        if (string.IsNullOrWhiteSpace(sceneName)) return false;

        // Expected names: "Level_1", "Level_2", ...
        if (!sceneName.StartsWith("Level_", StringComparison.OrdinalIgnoreCase)) return false;

        var suffix = sceneName.Substring("Level_".Length);
        if (!int.TryParse(suffix, out int levelNum)) return false;

        // Map Level_9 (actual level 9) to index 10 because BonusLevel is 9
        if (levelNum >= 9)
        {
            levelIndex = levelNum + 1;
        }
        else
        {
            levelIndex = levelNum;
        }

        return levelIndex >= MinLevelIndex && levelIndex <= MaxLevelIndex;
    }
}

