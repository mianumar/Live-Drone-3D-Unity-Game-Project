using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject Video,Tpanal;

    void Start()
    {
        // Ensure Video and taptostart are initialized but Video is initially inactive
        Video.SetActive(false);
    }

    public void getStart()
    {
        Video.SetActive(true);
        Tpanal.SetActive(false);
    }

    public void startgame()
    {
        // Load Level_8 scene
        SceneManager.LoadScene("Level_1");
    }
}
