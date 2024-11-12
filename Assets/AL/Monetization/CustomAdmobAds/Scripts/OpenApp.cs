using System;
using System.Collections;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace AL.Monetization.CustomAdmobAds.Scripts
{
    public class OpenApp : AdController
    {
        AppOpenAd appOpenAd;
        // These ad units are configured to always serve test ads.
        [FormerlySerializedAs("AdLoadedStatus")] public bool adLoadedStatus;
        public string adUnitIdAndroid = "ca-app-pub-3940256099942544/3419835294";
        private string adUnitIdIOS = "ca-app-pub-3940256099942544/5662855259";


        public string _adUnitId;
        // Start is called before the first frame update
        public static OpenApp instance;
        private void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        
#if UNITY_ANDROID
            _adUnitId = adUnitIdAndroid;
#elif UNITY_IPHONE
            _adUnitId = adUnitIdIOS;
#endif
        }

        protected override void CheckAdLoad()
        {
            if (adLoadedStatus)
            {
                return;
            }
            else
            {
                LoadAppOpenAd();
            }
        }

        void Start()
        {
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                LoadAppOpenAd();
            });
        }

        private void OnDestroy()
        {
            AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
        }


        // Update is called once per frame
 
        private void OnEnable()
        {
            base.OnEnable();
        }
        

        public void LoadAppOpenAd()
        {
            // Clean up the old ad before loading a new one.
            if (appOpenAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading the app open ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest.Builder().Build();

            // send the request to load the ad.
            AppOpenAd.Load(_adUnitId, ScreenOrientation.Portrait, adRequest,
                (AppOpenAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("app open ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("App open ad loaded with response : "
                              + ad.GetResponseInfo());

                    appOpenAd = ad;
                    RegisterEventHandlers(ad);
                    adLoadedStatus=(true);
                });
        }

        private void RegisterEventHandlers(AppOpenAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
              
                Debug.Log(String.Format("App open ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("App open ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("App open ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("App open ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("App open ad full screen content closed.");
                LoadAppOpenAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("App open ad failed to open full screen content " +
                               "with error : " + error);
                LoadAppOpenAd();

            };
        }

        private void OnAppStateChanged(AppState state)
        {
            Debug.Log("App State changed to : " + state);

            // if the app is Foregrounded and the ad is available, show it.
            if (state == AppState.Foreground)
            {
                StartCoroutine(ShowAppOpenAdWithDelay());
            }
        }

        IEnumerator ShowAppOpenAdWithDelay()
        {
            yield return new WaitForSeconds(0.2f);
            if (PlayerPrefs.GetInt("openApp") % 2 == 0)
            {
                ShowAppOpenAd();
                Debug.Log("aaa");
            }
            PlayerPrefs.SetInt("openApp",PlayerPrefs.GetInt("openApp")+1);
      
        }

        public void ShowAppOpenAd()
        {
            if (appOpenAd != null && appOpenAd.CanShowAd())
            {
                Debug.Log("Showing app open ad.");
                appOpenAd.Show();
            
                // TimerAd.instance. timer =  TimerAd.instance.delay;
            }
            else
            {
                Debug.LogError("App open ad is not ready yet.");
            }
            adLoadedStatus=(false);
        }
        public void DestroyAd()
        {
            if (appOpenAd != null)
            {
                Debug.Log("Destroying app open ad.");
                appOpenAd.Destroy();
                appOpenAd = null;
            }

            // Inform the UI that the ad is not ready.
            adLoadedStatus=(false);
        }
    }
}