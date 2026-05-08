/*using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public Transform content;
    public GameObject rowPrefab;

    List<ScoreData> scores = new List<ScoreData>();

    public void Load()
    {
        Debug.Log("Fetching leaderboard...");

        FirebaseDatabase.DefaultInstance.GetReference("HighScores")
        .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Firebase Data Received");

                if (!task.Result.Exists)
                {
                    Debug.Log("❌ No data in Firebase!");
                    return;
                }

                foreach (var child in task.Result.Children)
                {
                    Debug.Log(child.GetRawJsonValue());
                }
            }
            else
            {
                Debug.LogError("❌ Firebase Fetch Failed");
            }
        });
    }

    void Display()
    {
        foreach (Transform t in content) Destroy(t.gameObject);

        string myId = PlayerData.Instance.userId;
        int myRank = -1;

        for (int i = 0; i < scores.Count; i++)
        {
            if (scores[i].id == myId)
            {
                myRank = i + 1;
                break;
            }
        }

        for (int i = 0; i < Mathf.Min(5, scores.Count); i++)
        {
            CreateRow(i + 1, scores[i], false);
        }

        if (myRank > 5)
        {
            CreateRow(myRank, scores[myRank - 1], true);
        }
    }

    void CreateRow(int rank, ScoreData data, bool highlight)
    {
        GameObject obj = Instantiate(rowPrefab, content);
        obj.GetComponent<LeaderboardRow>().Set(rank, data.name, data.score, highlight);
        Debug.Log($"Creating Row: {data.name} - {data.score}");
    }
}

[System.Serializable]
public class ScoreData
{
    public string id;
    public string name;
    public int score;
}*/