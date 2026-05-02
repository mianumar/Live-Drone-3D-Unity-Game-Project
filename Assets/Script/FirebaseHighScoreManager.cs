/*using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseHighScoreManager : MonoBehaviour
{
    public static FirebaseHighScoreManager Instance;
    private DatabaseReference dbReference;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();
        }
        else { Destroy(gameObject); }
    }

    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase Database Initialized");
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    public void UpdateHighScore(int score)
    {
        if (dbReference == null) return;

        string userId = PlayerData.Instance.userId;

        DatabaseReference userRef = dbReference.Child("HighScores").Child(userId);

        userRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                int oldScore = 0;

                if (task.Result.Exists && task.Result.Child("score").Value != null)
                {
                    int.TryParse(task.Result.Child("score").Value.ToString(), out oldScore);
                }

                if (score > oldScore)
                {
                    var data = new
                    {
                        id = userId,
                        name = PlayerData.Instance.playerName,
                        score = score
                    };

                    string json = JsonUtility.ToJson(data);

                    userRef.SetRawJsonValueAsync(json);
                }
            }
        });
    }
}*/