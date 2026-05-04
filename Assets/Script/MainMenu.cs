using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using AL.Monetization.CustomAdmobAds.Scripts;

public class MainMenu : MonoBehaviour
{
    public GameObject Video,Tpanal,inputpanel;

    void Start()
    {
        // Ensure Video and taptostart are initialized but Video is initially inactive
        Video.SetActive(false);
    }

    public void getStart()
    {
        //inputpanel.SetActive(false);
        Video.SetActive(true);
        Tpanal.SetActive(false);
    }

    public void startgame()
    {
        // Load Level_8 scene
        //SceneManager.LoadScene("Level_1");
        ContinueGame();
    }

    // Hook this to a "Continue" button if you have one.
    public void ContinueGame()
    {
        if (ProgressManager.Instance != null)
        {
            SceneManager.LoadScene(ProgressManager.Instance.GetContinueSceneName());
        }
        else
        {
            SceneManager.LoadScene("Level_1");
        }
    }
    public void ShowInterstitial()
    {
        AdManager.instance.ShowInterstitial(); // paste this line anywhere you want to show interstitial 
    }
}
