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
        FirebaseDatabase.DefaultInstance.GetReference("HighScores")
        .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            scores.Clear();

            foreach (var child in task.Result.Children)
            {
                string json = child.GetRawJsonValue();
                ScoreData data = JsonUtility.FromJson<ScoreData>(json);
                scores.Add(data);
            }

            scores.Sort((a, b) => b.score.CompareTo(a.score));

            Display();
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
    }
}

[System.Serializable]
public class ScoreData
{
    public string id;
    public string name;
    public int score;
}*/