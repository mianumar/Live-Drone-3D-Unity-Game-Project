using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace AL.Utility.InternetCheckAvailability.Scripts
{
    public class InternetManager : MonoBehaviour
    {
        public static InternetManager instance;
        public GameObject noInternetCanvas;
        public Action loadAdAfterInternet;
        
        // Multiple URLs to check in case one is blocked or inaccessible
        [SerializeField] private string[] _connectivityCheckUrls = new string[] 
        {
            "https://www.google.com/",
            "https://www.apple.com/",
            "https://www.microsoft.com/"
        };
        
        [SerializeField] private float _checkInterval = 3f;
        public bool internetAvailable;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            StartCoroutine(CheckRoutine());
        }
    
        IEnumerator CheckRoutine()
        {
            while (true)
            {
                bool hasConnection = false;
                
                // First try the reliable Application.internetReachability method
                if (Application.internetReachability != NetworkReachability.NotReachable)
                {
                    hasConnection = true;
                    Debug.Log("Internet detected via NetworkReachability");
                }
                else
                {
                    // Fall back to web requests to multiple URLs if NetworkReachability fails
                    for (int i = 0; i < _connectivityCheckUrls.Length; i++)
                    {
                        using (UnityWebRequest request = UnityWebRequest.Head(_connectivityCheckUrls[i]))
                        {
                            request.timeout = 5; // Set a timeout of 5 seconds
                            
                            yield return request.SendWebRequest();
                            
                            if (!request.isNetworkError && !request.isHttpError)
                            {
                                hasConnection = true;
                                Debug.Log($"Internet connectivity confirmed via {_connectivityCheckUrls[i]}");
                                break;
                            }
                        }
                        
                        yield return null;
                    }
                }
 
                if (hasConnection)
                {
                    Debug.Log("Internet connection available");
                    if (!internetAvailable)
                    {
                        Debug.Log("Internet recovered - triggering ad reloads");
                        loadAdAfterInternet?.Invoke();
                    }
                    internetAvailable = true;
                    noInternetCanvas.SetActive(false);
                    Time.timeScale = 1;
                }
                else
                {
                    internetAvailable = false;
                    Debug.Log("No internet connection detected");
                    noInternetCanvas.SetActive(true);
                    Time.timeScale = 0;
                }
          
                yield return new WaitForSecondsRealtime(_checkInterval);
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                StopAllCoroutines();
                StartCoroutine(CheckRoutine());
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                StopAllCoroutines();
                StartCoroutine(CheckRoutine());
            }
        }
    }
}