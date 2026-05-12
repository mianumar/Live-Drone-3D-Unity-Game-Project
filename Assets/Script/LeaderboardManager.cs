using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LeaderboardManager : MonoBehaviour
{
    public Transform content;
    public GameObject rowPrefab;

    private string getUrl = "https://faizcreations.com/droneGameApi/get_leaderboard.php";

    // This matches the JSON structure from the PHP script
    [System.Serializable]
    public class LeaderboardResponse
    {
        public List<ScoreData> topPlayers;
        public int userRank;
        public string userName;
        public int userScore;
    }

    private void Start()
    {
        Load();
    }

    public void Load()
    {
        Debug.Log("Fetching SQL leaderboard...");
        StartCoroutine(FetchLeaderboard());
    }

    private IEnumerator FetchLeaderboard()
    {
        // We pass the userId so the PHP script can find our specific rank
        string url = $"{getUrl}?userId={PlayerData.Instance.userId}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("SQL Data Received: " + www.downloadHandler.text);
                LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(www.downloadHandler.text);
                Display(response);
            }
            else
            {
                Debug.LogError("❌ SQL Fetch Failed: " + www.error);
            }
        }
    }

    void Display(LeaderboardResponse data)
    {
        // Clear old rows
        foreach (Transform t in content) Destroy(t.gameObject);

        bool userInTop7 = false;

        // 1. Display Top 7
        for (int i = 0; i < data.topPlayers.Count; i++)
        {
            int rank = i + 1;
            bool isMe = data.topPlayers[i].name == data.userName && data.topPlayers[i].score == data.userScore;

            if (isMe) userInTop7 = true;

            CreateRow(rank, data.topPlayers[i].name, data.topPlayers[i].score, isMe);
        }

        // 2. If user is not in Top 7, add them as the 8th row
        if (!userInTop7 && data.userRank > 0)
        {
            // Optional: add a "..." separator if you want
            CreateRow(data.userRank, data.userName, data.userScore, true);
        }
    }

    void CreateRow(int rank, string name, int score, bool highlight)
    {
        GameObject obj = Instantiate(rowPrefab, content);
        obj.GetComponent<LeaderboardRow>().Set(rank, name, score, highlight);
    }
}

[System.Serializable]
public class ScoreData
{
    public string id;
    public string name;
    public int score;
}