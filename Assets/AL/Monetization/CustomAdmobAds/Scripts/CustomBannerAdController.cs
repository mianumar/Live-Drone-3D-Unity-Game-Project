using System;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Serialization;

namespace AL.Monetization.CustomAdmobAds.Scripts
{
    public class CustomBannerAdController : AdController
    {
        [FormerlySerializedAs("AdLoadedStatus")] public bool adLoadedStatus;

        public bool customBanner;
        public int horizontalSize=250,verticalSize=250;
        public AdPosition position;
        public string adIdAndroid = "ca-app-pub-3940256099942544/6300978111";
        public string _adIdIOS = "ca-app-pub-3940256099942544/2934735716";
        private string _adUnitId;
        private BannerView _bannerView;
        public bool customPosition;
        public Vector2 bannerPos;
        public void Awake()
        {
#if UNITY_ANDROID
            _adUnitId = adIdAndroid;
#elif UNITY_IPHONE
            _adUnitId = _adIdIOS;
#endif
            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                // This callback is called once the MobileAds SDK is initialized.
            });
        }
        private void OnEnable()
        {
            base.OnEnable();
            ShowBanner();
        }

        protected override void CheckAdLoad()
        {
            if (adLoadedStatus)
            {
                return;
            }
            else
            {
                LoadAd();
            }
        }
        public void Start()
        {
            // Initialize the Google Mobile Ads SDK.
          
            // 
        }
        /// <summary>
        /// Creates a 320x50 banner at top of the screen.
        /// </summary>
        public void CreateBannerView()
        {
            Debug.Log("Creating banner view");

            // If we already have a banner, destroy the old one.
            if (_bannerView != null)
            {
                DestroyAd();
            }

            if (customBanner)
            {
                AdSize adSize = new AdSize(horizontalSize, verticalSize);
                if (customPosition)
                {
                    _bannerView = new BannerView(_adUnitId, adSize, (int)bannerPos.x,(int)bannerPos.y);
                }
                else
                {
                    _bannerView = new BannerView(_adUnitId, adSize, position);
                }
            }
            else
            {
                _bannerView = new BannerView(_adUnitId, AdSize.Banner, position);
            }

            ListenToAdEvents();
            // Create a 320x50 banner at top of the screen
        }
        public void LoadAd()
        {
            // create an instance of a banner view first.
            
            if(_bannerView == null)
            {
                CreateBannerView();
            }
            // create our request used to load the ad.
            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");

            // send the request to load the ad.
            Debug.Log("Loading banner ad.");
            _bannerView.LoadAd(adRequest);
        }

        public void ShowBanner()
        {
            LoadAd();
            _bannerView.Show();
        }

        public void HideBanner()
        {
            _bannerView.Hide();
        }
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
        public void DestroyAd()
        {
            if (_bannerView != null)
            {
                Debug.Log("Destroying banner ad.");
                _bannerView.Destroy();
                _bannerView = null;
            }
            adLoadedStatus = false;
        }
    }
}