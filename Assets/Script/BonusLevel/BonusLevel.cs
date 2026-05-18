using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class BonusLevel : MonoBehaviour
{
    public static BonusLevel instance;

    [Header("Checkpoint Settings")]
    public GameObject checkpoint; // Assign your Checkpoint object here in the Inspector

    [Header("UI")]
    public TMP_Text plus10Text;
    public GameObject levelCompletePanel;

    [Header("Animation")]
    public float moveSpeed = 100f;
    public float duration = 0.5f;

    private Vector3 startPos;
    private Color originalColor;

    private void Awake()
    {
        instance = this;

        if (plus10Text != null)
        {
            startPos = plus10Text.transform.localPosition;
            originalColor = plus10Text.color;
            plus10Text.gameObject.SetActive(false);
        }

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }
    }

    // This handles collisions for BOTH Coins and the Checkpoint
    private void OnTriggerEnter(Collider other)
    {
        // This will print EVERY time the player touches ANY trigger
        Debug.Log("Collision Detected with: " + other.name + " | Tag: " + other.tag);

        if (other.CompareTag("Player"))
        {
            if (ProgressManager.Instance != null)
            {
                ProgressManager.Instance.MarkCurrentSceneLevelComplete();
            }

            Debug.Log("Player tag confirmed!");

            if (levelCompletePanel != null)
            {
                int score = ScoreManager.instance.score;
                PlayerData.Instance.AddToTotalScore(score, 1);
                levelCompletePanel.SetActive(true);
                Time.timeScale = 0f;
                Debug.Log("Panel should be visible now.");
            }
            else
            {
                Debug.LogError("LEVEL COMPLETE PANEL IS NOT ASSIGNED IN INSPECTOR!");
            }
        }
    }

    private void CompleteLevel()
    {

        PlayerPrefs.SetInt("UnlockedLevel", 10);
        PlayerPrefs.Save();
        Debug.Log($"Progress saved. Unlocked level is now: Index : {10}");


        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            Time.timeScale = 0f; // Pause game
        }
    }

    // COIN ANIMATION
    public void ShowPlus10()
    {
        if (plus10Text == null) return;
        StopAllCoroutines();
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        plus10Text.gameObject.SetActive(true);
        plus10Text.transform.localPosition = startPos;

        Color color = originalColor;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            plus10Text.transform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;
            color.a = Mathf.Lerp(originalColor.a, 0f, timer / duration);
            plus10Text.color = color;
            yield return null;
        }

        plus10Text.gameObject.SetActive(false);
        plus10Text.transform.localPosition = startPos;
        plus10Text.color = originalColor;
    }
}