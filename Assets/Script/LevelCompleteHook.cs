using UnityEngine;

public class LevelCompleteHook : MonoBehaviour
{
    public LevelTimerScore timer;

    void OnEnable()
    {
        // This runs when levelCompletePanel becomes active
        timer.OnLevelComplete();
    }
}