using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class ProgressManager : MonoBehaviour
{
    private const string UnlockedLevelKey = "UnlockedLevel";

    // Keep these in sync with your actual scenes.
    public const int MinLevelIndex = 1;
    public const int MaxLevelIndex = 16;

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
        MarkLevelComplete(SceneManager.GetActiveScene().name);
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

    public string GetContinueSceneName()
    {
        // Continue at the highest unlocked level.
        return $"Level_{GetUnlockedLevel()}";
    }

    private static bool TryParseLevelIndex(string sceneName, out int levelIndex)
    {
        levelIndex = 0;
        if (string.IsNullOrWhiteSpace(sceneName)) return false;

        // Expected names: "Level_1", "Level_2", ...
        if (!sceneName.StartsWith("Level_", StringComparison.OrdinalIgnoreCase)) return false;

        var suffix = sceneName.Substring("Level_".Length);
        if (!int.TryParse(suffix, out levelIndex)) return false;

        return levelIndex >= MinLevelIndex && levelIndex <= MaxLevelIndex;
    }
}

