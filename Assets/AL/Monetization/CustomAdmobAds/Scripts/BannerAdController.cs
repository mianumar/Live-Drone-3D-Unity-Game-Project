using System;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Serialization;

namespace AL.Monetization.CustomAdmobAds.Scripts
{
    public class BannerAdController : AdController
    {
        
        // Start is called before the first frame update
        [FormerlySerializedAs("AdLoadedStatus")] public bool adLoadedStatus;
        public string adIdAndroid = "ca-app-pub-3940256099942544/6300978111";
        public string _adIdIOS = "ca-app-pub-3940256099942544/2934735716";
        private string _adUnitId;
        private BannerView _bannerView;
        public AdPosition position;


        private void Awake()
        {
#if UNITY_ANDROID
            _adUnitId = adIdAndroid;
#elif UNITY_IPHONE
            _adUnitId = _adIdIOS;
#endif
        }

        protected override void CheckAdLoad()
        {
            if (adLoadedStatus)
            {
                return;
            }
          
            LoadAd();
            
        }
        private void OnEnable()
        {
            base.OnEnable();
            LoadAd();
        }

        public void CreateBannerView()
        {
            Debug.Log("Creating banner view.");

            // If we already have a banner, destroy the old one.
            if(_bannerView != null)
            {
                DestroyAd();
            }

            // Create a 320x50 banner at top of the screen.
            _bannerView = new BannerView(_adUnitId, AdSize.Banner, position);

            // Listen to events the banner may raise.
            ListenToAdEvents();

            Debug.Log("Banner view created.");
        }
        public void LoadAd()
        {
            // Create an instance of a banner view first.
            if(_bannerView == null)
            {
                CreateBannerView();
            }

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            Debug.Log("Loading banner ad.");
            _bannerView.LoadAd(adRequest);
        }
        public void ShowAd()
        {
            if (_bannerView != null)
            {
                Debug.Log("Showing banner view.");
                _bannerView.Show();
            }
        }
        public void HideAd()
        {
            if (_bannerView != null)
            {
                Debug.Log("Hiding banner view.");
                _bannerView.Hide();
            }
        }
        public void DestroyAd()
        {
            if (_bannerView != null)
            {
                Debug.Log("Destroying banner view.");
                _bannerView.Destroy();
                _bannerView = null;
            }

            // Inform the UI that the ad is not ready.
            adLoadedStatus = false;
        }
        // public void LogResponseInfo()
        // {
        //     if (_bannerView != null)
        //     {
        //         var responseInfo = _bannerView.GetResponseInfo();
        //         if (responseInfo != null)
        //         {
        //             UnityEngine.Debug.Log(responseInfo);
        //         }
        //     }
        // }
        private void ListenToAdEvents()
        {
            // Raised when an ad is loaded into the banner view.
            _bannerView.OnBannerAdLoaded += () =>
            {
                Debug.Log("Banner view loaded an ad with response : "
                          + _bannerView.GetResponseInfo());
    
                // Inform the UI that the ad is ready.
                adLoadedStatus=(true);
            };
            // Raised when an ad fails to load into the banner view.
            _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                Debug.LogError("Banner view failed to load an ad with error : " + error);
            };
            // Raised when the ad is estimated to have earned money.
            _bannerView.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Banner view paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            _bannerView.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Banner view recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            _bannerView.OnAdClicked += () =>
            {
                Debug.Log("Banner view was clicked.");
            };
            // Raised when an ad opened full screen content.
            _bannerView.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Banner view full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            _bannerView.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Banner view full screen content closed.");
            };
        }

        /// <summary>
        /// Hides the ad.
        /// </summary>
       

    }
}