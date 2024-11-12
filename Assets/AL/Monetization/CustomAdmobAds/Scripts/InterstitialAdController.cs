using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace AL.Monetization.CustomAdmobAds.Scripts
{
    public class InterstitialAdController : AdController
    {
        public bool AdLoadedStatus;
        // Start is called before the first frame update
        public string adIdAndroid = "ca-app-pub-3940256099942544/6300978111";
        public string _adIdIOS = "ca-app-pub-3940256099942544/2934735716";
        private string _adUnitId;
        private InterstitialAd _interstitialAd;
        private Action _rewardAction;
        
        private void Awake()
        {
            
        
#if UNITY_ANDROID
            _adUnitId = adIdAndroid;
#elif UNITY_IPHONE
            _adUnitId = _adIdIOS;
#endif
        }
        
        private void OnEnable()
        {
            base.OnEnable();
            LoadAd();
        }
      

        protected override void CheckAdLoad()
        {
            if (AdLoadedStatus)
            {
                return;
            }
            else
            {
                LoadAd();
            }
        }

        public void ShowAd()
        {
            ShowAd(null);
        }
        
        public void ShowAd(Action rewardAction)
        {
            this._rewardAction = rewardAction;
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {
                Debug.Log("Showing interstitial ad.");
                _interstitialAd.Show();
                rewardAction?.Invoke();
                Debug.Log("AL Intertitial Showed");
            }
            else
            {
                LoadAd();
                Debug.LogError("Interstitial ad is not ready yet.");
            }
         
            // Inform the UI that the ad is not ready.
        }
        public void LogResponseInfo()
        {
            if (_interstitialAd != null)
            {
                var responseInfo = _interstitialAd.GetResponseInfo();
                UnityEngine.Debug.Log(responseInfo);
            }
        }

       

      
        
        public void DestroyAd()
        {
            if (_interstitialAd != null)
            {
                Debug.Log("Destroying interstitial ad.");
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            // Inform the UI that the ad is not ready.
            AdLoadedStatus=false;
        }
        public void LoadAd()
        {
            // Clean up the old ad before loading a new one.
            if (_interstitialAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading interstitial ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            InterstitialAd.Load(_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
            {
                // If the operation failed with a reason.
                if (error != null)
                {
                    Debug.LogError("Interstitial ad failed to load an ad with error : " + error);
                    return;
                }
                // If the operation failed for unknown reasons.
                // This is an unexpected error, please report this bug if it happens.
                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                    return;
                }

                // The operation completed successfully.
                Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
                _interstitialAd = ad;

                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);

                // Inform the UI that the ad is ready.
                AdLoadedStatus=true;
            });
        }
        private void RegisterEventHandlers(InterstitialAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Interstitial ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("Interstitial ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Interstitial ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
              
                Debug.Log("Interstitial ad full screen content closed.");
                LoadAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
            
                Debug.LogError("Interstitial ad failed to open full screen content with error : "
                               + error);
                LoadAd();
            };
        }
    }
}