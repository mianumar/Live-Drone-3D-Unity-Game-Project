using AL.Monetization.CustomAdmobAds.Scripts;
using UnityEngine;

public class DummyCustomRewardedVideo : MonoBehaviour
{
    public AdManager rController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void ShowReward()
    {
        rController.ShowRewardedVideo(Reward);
    }

    private void Reward()
    {
        Debug.Log("lalalal");
    }
}
