using System.Collections;
using System.Collections.Generic;
using AL_CustomAdmobAds.Scripts;
using AL.Monetization.CustomAdmobAds.Scripts;
using UnityEngine;

public class Dummy2 : MonoBehaviour
{
    public RewardedInterstitialAdController rController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void ShowReward()
    {
        rController.ShowAd(Reward);
    }

    private void Reward()
    {
        Debug.Log("aaaaaa");
    }
}
