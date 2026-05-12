using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SQLHighScoreManager : MonoBehaviour
{
    public static SQLHighScoreManager Instance;

    // Updated URL pointing to your new droneGameApi subfolder
    private string updateScoreURL = "https://faizcreations.com/droneGameApi/update_score.php";

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
        }
    }

    /// <summary>
    /// Call this from ScoreManager.LevelComplete to send the score to your website.
    /// </summary>
    public void UpdateHighScore(int score)
    {
        StartCoroutine(PostScore(score));
    }

    private IEnumerator PostScore(int score)
    {
        // Prepare the form data to match the PHP $_POST keys
        WWWForm form = new WWWForm();
        form.AddField("userId", PlayerData.Instance.userId);
        form.AddField("playerName", PlayerData.Instance.playerName);
        form.AddField("score", score);

        using (UnityWebRequest www = UnityWebRequest.Post(updateScoreURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Database Error: " + www.error);
            }
            else
            {
                // Shows "Score updated successfully" or "New record created" in Console
                Debug.Log("Server Response: " + www.downloadHandler.text);
            }
        }
    }
}