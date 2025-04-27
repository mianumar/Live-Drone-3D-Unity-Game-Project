using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;

namespace AL.Monetization.CustomAdmobAds.Scripts
{
    public class InterstitialAdController : AdController
    {
        public bool AdLoadedStatus;
        
        [Header("Ad Unit IDs")]
        [SerializeField] private string adIdAndroid = "ca-app-pub-8397648328712036/1245446676";
        [SerializeField] private string _adIdIOS = "ca-app-pub-3940256099942544/2934735716";
        
        [Header("Settings")]
        [SerializeField] private bool _autoReloadOnClose = true;
        [SerializeField] private float _retryDelay = 5f;
        [SerializeField] private int _maxRetryCount = 3;
        
        private string _adUnitId;
        private InterstitialAd _interstitialAd;
        private Action _rewardAction;
        private int _retryCount = 0;
        private bool _isLoading = false;
        private bool _hasLoadFailed = false;
        
        private void Awake()
        {
#if UNITY_ANDROID
            _adUnitId = adIdAndroid;
            Debug.Log($"Using Android Ad Unit ID: {_adUnitId}");
#elif UNITY_IPHONE
            _adUnitId = _adIdIOS;
            Debug.Log($"Using iOS Ad Unit ID: {_adUnitId}");
#else
            _adUnitId = "unused";
            Debug.Log("Neither Android nor iOS platform detected");
#endif
        }
        
        private void OnEnable()
        {
            base.OnEnable();
            
            // Wait for AdMob to initialize before loading ads
            if (AdMobInitializer.Instance != null)
            {
                if (AdMobInitializer.Instance.IsInitialized)
                {
                    LoadAd();
                }
                else
                {
                    AdMobInitializer.Instance.OnInitializationComplete += OnAdMobInitialized;
                }
            }
            else
            {
                // Fallback to direct loading if AdMobInitializer doesn't exist
                Debug.LogWarning("AdMobInitializer not found - attempting direct ad loading.");
                LoadAd();
            }
        }
        
        private void OnDisable()
        {
            base.OnDisable();
            
            if (AdMobInitializer.Instance != null)
            {
                AdMobInitializer.Instance.OnInitializationComplete -= OnAdMobInitialized;
            }
        }
        
        private void OnAdMobInitialized()
        {
            LoadAd();
        }

        protected override void CheckAdLoad()
        {
            if (AdLoadedStatus || _isLoading)
            {
                return;
            }
            
            LoadAd();
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
                Debug.Log("AL Interstitial Showed");
            }
            else
            {
                Debug.LogWarning("Interstitial ad is not ready yet. Attempting to load a new one.");
                _hasLoadFailed = true;
                LoadAd();
                
                // Still invoke the callback even if we couldn't show the ad
                // to avoid blocking game flow
                rewardAction?.Invoke();
            }
        }
        
        public void LogResponseInfo()
        {
            if (_interstitialAd != null)
            {
                var responseInfo = _interstitialAd.GetResponseInfo();
                Debug.Log($"Ad Response Info: {responseInfo}");
            }
            else
            {
                Debug.Log("No ad to get response info from.");
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

            AdLoadedStatus = false;
        }
        
        public void LoadAd()
        {
            if (_isLoading)
            {
                Debug.Log("Ad is already loading, waiting...");
                return;
            }
            
            // Clean up the old ad before loading a new one.
            if (_interstitialAd != null)
            {
                DestroyAd();
            }

            Debug.Log($"Loading interstitial ad with ID: {_adUnitId}");
            _isLoading = true;
            
            // Create our request used to load the ad.
            var adRequest = new AdRequest.Builder().Build();

            // Send the request to load the ad.
            InterstitialAd.Load(_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
            {
                _isLoading = false;
                
                // If the operation failed with a reason.
                if (error != null)
                {
                    Debug.LogError($"Interstitial ad failed to load with error: {error.GetMessage()}, Code: {error.GetCode()}");
                    
                    // Retry loading after delay
                    if (_retryCount < _maxRetryCount && _hasLoadFailed)
                    {
                        _retryCount++;
                        Debug.Log($"Retrying ad load ({_retryCount}/{_maxRetryCount}) in {_retryDelay} seconds...");
                        StartCoroutine(RetryLoadCoroutine());
                    }
                    else if (_retryCount >= _maxRetryCount)
                    {
                        Debug.LogWarning($"Max retry count reached ({_maxRetryCount}). Giving up on loading interstitial ad.");
                        _retryCount = 0;
                    }
                    
                    return;
                }
                
                // If the operation failed for unknown reasons.
                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                    return;
                }

                // The operation completed successfully.
                Debug.Log("Interstitial ad loaded successfully!");
                _interstitialAd = ad;
                _retryCount = 0;
                _hasLoadFailed = false;

                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);

                // Inform the UI that the ad is ready.
                AdLoadedStatus = true;
            });
        }
        
        private IEnumerator RetryLoadCoroutine()
        {
            yield return new WaitForSeconds(_retryDelay);
            LoadAd();
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
                
                // Invoke reward action when ad is shown
                _rewardAction?.Invoke();
            };
            
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial ad full screen content closed.");
                
                // Reload ad if configured to do so
                if (_autoReloadOnClose)
                {
                    LoadAd();
                }
            };
            
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError($"Interstitial ad failed to open full screen content with error: {error.GetMessage()}, Code: {error.GetCode()}");
                
                // Invoke reward action even on error to prevent blocking game flow
                _rewardAction?.Invoke();
                
                if (_autoReloadOnClose)
                {
                    LoadAd();
                }
            };
        }
    }
}