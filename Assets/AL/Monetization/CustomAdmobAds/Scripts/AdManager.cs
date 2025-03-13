using System;
using AL_CustomAdmobAds.Scripts;
using UnityEngine;

namespace AL.Monetization.CustomAdmobAds.Scripts
{
    public class AdManager : MonoBehaviour
    {
        public static AdManager instance;
        // Start is called before the first frame update
        public RewardedInterstitialAdController rewardedInterstitialAdController;
        public RewardedAdController rewardedAdController;
        public InterstitialAdController interstitialAdController;
        public BannerAdController [] banners;
        public int bannerToShow=2;
        public Action showBanner;
        public Action hideBanner;
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
        }

        private void Start()
        {
            LoadBanner();
        }

        public void LoadBanner()
        {
            for (int i = 0; i < bannerToShow; i++)
            {
                banners[i].gameObject.SetActive(true);
            }
        }
        
        public void ShowBanner()
        {
            showBanner?.Invoke();
        }
        public void HideBanner()
        {
            hideBanner?.Invoke();
        }
        public void ShowRewardedVideo(Action rewardedAction)
        {
            if (rewardedAdController.AdLoadedStatus)
            {
                rewardedAdController.ShowAd(rewardedAction);
            }
            else if(rewardedInterstitialAdController.AdLoadedStatus)
            {
                rewardedInterstitialAdController.ShowAd(rewardedAction);
            }
            else
            {
                interstitialAdController.ShowAd(rewardedAction);
            }
        }
        public void ShowInterstitial()
        {
            interstitialAdController.ShowAd(() =>
            {

            });
        }
    }
}