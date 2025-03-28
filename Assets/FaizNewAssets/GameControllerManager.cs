using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameControllerManager : MonoBehaviour
{
    [Header("UI ELEMENTS")]
    public GameObject welcomePanel;
    public GameObject TutorialPanel;
    public GameObject ScreenPanel;
    public Image[] nextImagesIcon;
    public Sprite SelectIcon;

    public AudioSource WELCOME_TO_DRONE_LIFE;


    [Header("CONTROLLER SETTINGS")]
    private Gamepad gamepad;
    public MainMenu mainMenu;


    public InputActionAsset inputActions; // Input Actions Asset to reference your controls
    private InputAction menuNextAction;
    //GamepadControllers controller;



    private void Awake()
    {
        menuNextAction = inputActions.FindAction("MenuNext");
        menuNextAction.performed += ctx => NextBtnPressed();


        //controller = new GamepadControllers();

        // controller.Controller.MenuNext.performed += ctx => MenuNextBtn();


    }
    void Start()
    {
        if (IsControllerConnected())
        {
            Debug.Log("Game controller is connected!"); 
            DontDestroyOnLoad(gameObject);
        }
        else
        {

        }

        menuNextAction.Enable();
    }

    public void NextBtnPressed()
    {
        if (welcomePanel != null && welcomePanel.activeSelf)
        {
            ShowTutorialPanel();
            Debug.Log("ShowTutorialPanel()");
        }
        else if (mainMenu.Tpanal != null && mainMenu.Tpanal.activeSelf) 
        {
            ShowVideoPanel();
            Debug.Log("ShowVideoPanel()");
        }
        else if (ScreenPanel != null && ScreenPanel.activeSelf)
        {
            ScreenPanelShow();
            Debug.Log("ScreenPanelShow()");
        }
        else
        {
            Debug.Log("ELSE");
        }

    }

    public void ShowTutorialPanel()
    {
        Debug.Log("ShowTutorialPanel");
        welcomePanel.SetActive(false);
        mainMenu.Tpanal.SetActive(true);
        WELCOME_TO_DRONE_LIFE.enabled = true;
    }
    public void ScreenPanelShow()
    {
        Debug.Log("ScreenPanelShow");
        mainMenu.startgame();
    }
      
    public void ShowVideoPanel()
    {
        Debug.Log("ShowVideoPanel");
        mainMenu.getStart();
        ScreenPanel.SetActive(true);

        //CleanUpReferences();
    }

    void Update()
    {

        // Null check for panels to avoid errors
        //CheckAndDestroyPanel(ref welcomePanel);
       // CheckAndDestroyPanel(ref mainMenu.Tpanal);

    }

    // Function to check if a controller is connected
    bool IsControllerConnected()
    {
        // Check if any gamepad is connected using the Unity Input System
        if (Gamepad.all.Count > 0)
        {
            // Log all connected gamepads
            foreach (var gamepad in Gamepad.all)
            {
                Debug.Log("Connected gamepad: " + gamepad.displayName);
            }

            foreach (var image in nextImagesIcon)
            {
                image.sprite = SelectIcon;
            }

            return true;
        }

        return false;
    }

    private void CleanUpReferences()
    {
        // Nullify or destroy any references that might cause problems after scene changes
        if (mainMenu != null)
        {
            if (mainMenu.Tpanal != null)
            {
                mainMenu.Tpanal.SetActive(false);
            }
            // Optionally, you could destroy mainMenu or other components if needed
        }
        else
        {
            Destroy(mainMenu);
        }

        // Nullify any UI elements that might cause errors
        if (welcomePanel != null && welcomePanel.activeSelf)
        {
            welcomePanel.SetActive(false);
        }

        if (ScreenPanel != null && ScreenPanel.activeSelf)
        {
            ScreenPanel.SetActive(false);
        }

        // Add any other cleanup logic here for other references that might cause issues
        // For example, destroy or nullify objects that are scene-dependent or not needed
    }

    private void CheckAndDestroyPanel(ref GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogWarning("Panel is null, destroying it.");
            Destroy(panel);  // Destroy the GameObject if it's null
        }
    }
}
