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

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            StartCoroutine(CheckRoutine());
        }
    

        // Start is called before the first frame 
        public bool internetAvailable;
        IEnumerator CheckRoutine()
        {
            while (true)
            {
            
                UnityWebRequest request = new UnityWebRequest("https://www.google.com/");
                yield return request.SendWebRequest();
 
                if (string.IsNullOrEmpty(request.error))
                {
                    Debug.Log("yes");
                    if (!internetAvailable)
                    {
                        loadAdAfterInternet?.Invoke();
                        Debug.Log("once");
                    }
                    internetAvailable = true;
                    noInternetCanvas.SetActive(false);
                    Time.timeScale = 1;

               
                }
                else
                {
                    internetAvailable = false;
                    Debug.Log("No");
                    noInternetCanvas.SetActive(true);
                    Time.timeScale = 0;
                }
          

                yield return new WaitForSecondsRealtime(1f);
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            StopAllCoroutines();
            StartCoroutine(CheckRoutine());
        }

        private void OnApplicationQuit()
        {
            StopAllCoroutines();
            StartCoroutine(CheckRoutine());
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            StopAllCoroutines();
            StartCoroutine(CheckRoutine());
        }
   
    }
}