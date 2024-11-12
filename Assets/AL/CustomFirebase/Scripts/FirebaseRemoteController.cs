using System;
using System.Threading.Tasks;

#if Firebase
using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
#endif

using UnityEngine;
using UnityEngine.Serialization;

namespace AL.CustomFirebase.Scripts{
    
    public class FirebaseRemoteController : MonoBehaviour
    {
        public static FirebaseRemoteController Instance;

        private void Awake()
        {

            Instance = this;
        }
#if Firebase

        [SerializeField] private  Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

        private void Start()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError(
                        "Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }

     

       


   private void InitializeFirebase()
        {

            Debug.Log("Remote config ready!");
            FetchFireBase();
        }
 private void FetchFireBase()
        {
            FetchDataAsync();

        }
          private Task FetchDataAsync()
        {
            Debug.Log("Fetching data...");
            System.Threading.Tasks.Task fetchTask =
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                    TimeSpan.Zero);
            return fetchTask.ContinueWithOnMainThread(FetchComplete);
        } 
        
         private void FetchComplete(Task fetchTask)
        {
            if (!fetchTask.IsCompleted)
            {
                Debug.LogError("Retrieval hasn't finished.");
                return;
            }

            var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            var info = remoteConfig.Info;
            if (info.LastFetchStatus != LastFetchStatus.Success)
            {
                Debug.LogError(
                    $"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
                return;
            }

            // Fetch successful. Parameter values must be activated to use.
            remoteConfig.ActivateAsync()
                .ContinueWithOnMainThread(
                    task => { Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}."); });
        }

#endif
     

       


        public bool GetBoolean(string variable)
        {
#if Firebase
           if (dependencyStatus == DependencyStatus.Available)
            {
                var showCompleteAds = FirebaseRemoteConfig.DefaultInstance.GetValue(variable).BooleanValue;
                Debug.Log("Ask for "+variable+" :" + showCompleteAds);
                return showCompleteAds;
            }

#endif
          
            Debug.Log("RemoteConfig not yet available ");
            return false;
        }
    }
}