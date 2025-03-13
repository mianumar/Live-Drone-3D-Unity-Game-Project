using AL_CustomAdmobAds.Scripts;
using AL.Monetization.CustomAdmobAds.Scripts;
using UnityEngine;

namespace AL.CustomAdmobAds.Scripts
{
    public class Dummy : MonoBehaviour
    {
        public RewardedAdController rController;
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
}
