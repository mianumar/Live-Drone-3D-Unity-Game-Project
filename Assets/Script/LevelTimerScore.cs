using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LevelTimerScore : MonoBehaviour
{
    public float timeElapsed = 0f;
    public bool isRunning = true;

    public int maxScore = 1000;
    public int penaltyRate = 10;

    public TextMeshProUGUI finalScoreText; // assign from your LevelCompletePanel
    public TMP_Text timerText;
    public TMP_Text scoreText; // assign from your LevelCompletePanel

    void Update()
    {
        if (isRunning)
        {
            timeElapsed += Time.deltaTime;
        }
        if (timerText != null)
            timerText.text = "Time: " + Mathf.FloorToInt(timeElapsed);
        if (scoreText != null)
            scoreText.text = "Score: " + CalculateScore().ToString("0");
    }

    float CalculateScore()
    {
        float score = maxScore - (timeElapsed * penaltyRate);
        return Mathf.Max(score, 0);
    }

    // 🔥 CALL THIS WHEN LEVEL COMPLETES
    public void OnLevelComplete()
    {
        isRunning = false;

        float finalScore = CalculateScore();
        if (timerText != null)
            timerText.gameObject.SetActive(false);

        if (scoreText != null)
            scoreText.gameObject.SetActive(false);
        // 👉 show in your existing panel
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + finalScore.ToString("0");
        }
    }
}