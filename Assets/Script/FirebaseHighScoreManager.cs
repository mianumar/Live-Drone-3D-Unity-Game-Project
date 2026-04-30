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

    public void UpdateHighScore(string userId, int score)
    {
        if (dbReference == null) return;

        // Path: HighScores / UserID / Score
        dbReference.Child("HighScores").Child(userId).SetValueAsync(score).ContinueWithOnMainThread(task => {
            if (task.IsCompleted) Debug.Log("Score updated in Firebase!");
        });
    }
}*/