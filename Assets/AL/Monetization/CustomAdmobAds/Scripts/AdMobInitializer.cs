using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;

namespace AL.Monetization.CustomAdmobAds.Scripts
{
    public class AdMobInitializer : MonoBehaviour
    {
        public static AdMobInitializer Instance { get; private set; }
        
        [SerializeField] private bool _debugMode = true;
        
        public bool IsInitialized { get; private set; }
        public event Action OnInitializationComplete;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAdMob();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAdMob()
        {
            IsInitialized = false;
            
            // Initialize the Google Mobile Ads SDK
            MobileAds.Initialize(initStatus => 
            {
                IsInitialized = true;
                
                if (_debugMode)
                {
                    // Get the adapter initialization status
                    var adapterStatusMap = initStatus.getAdapterStatusMap();
                    
                    foreach (var adapterStatus in adapterStatusMap)
                    {
                        string adaptorClassName = adapterStatus.Key;
                        var status = adapterStatus.Value;
                        
                        Debug.Log($"Adapter: {adaptorClassName}, Status: {status.InitializationState}, " +
                                  $"Description: {status.Description}, Latency: {status.Latency}ms");
                    }

                    // Get device info for debugging
                    Debug.Log($"Device Platform: {Application.platform}");
                    Debug.Log($"SystemInfo.deviceModel: {SystemInfo.deviceModel}");
                    Debug.Log($"SystemInfo.operatingSystem: {SystemInfo.operatingSystem}");
                    
                    // Request configuration settings
                    MobileAds.GetRequestConfiguration();
                }
                
                OnInitializationComplete?.Invoke();
            });
        }
        
        // Call this method for debug purposes
        public void LogAdMobStatus()
        {
            Debug.Log($"AdMob Initialization Status: {IsInitialized}");
            
            // Get adapter status if available
            var requestConfiguration = MobileAds.GetRequestConfiguration();
            Debug.Log($"Test Device IDs Count: {requestConfiguration.TestDeviceIds.Count}");
            
            foreach (var deviceId in requestConfiguration.TestDeviceIds)
            {
                Debug.Log($"Test Device ID: {deviceId}");
            }
        }
        
        // Add the current device as a test device
        public void AddTestDevice()
        {
            var requestConfiguration = new RequestConfiguration.Builder()
                .SetTestDeviceIds(new System.Collections.Generic.List<string>() 
                { 
                    SystemInfo.deviceUniqueIdentifier 
                })
                .build();
                
            MobileAds.SetRequestConfiguration(requestConfiguration);
            Debug.Log($"Added test device: {SystemInfo.deviceUniqueIdentifier}");
        }
    }
} 