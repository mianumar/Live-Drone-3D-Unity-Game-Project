using System.Collections;
using System.Collections.Generic;
using DroneController.Physics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    private InputAction moveAction; // To capture the movement (WASD like)
    private InputAction lookAction; // To capture rotation (Right Stick)
    private InputAction throttleUpAction; // To capture up movement (L2)
    private InputAction throttleDownAction; // To capture down movement (R2)


    //GamepadControllers controller;

    public DroneMovement droneMovementScript;




    private void Awake()
    {
        menuNextAction = inputActions.FindAction("MenuNext");
        moveAction = inputActions.FindAction("moveAction");
        //lookAction = inputActions.FindAction("Look");
        //throttleUpAction = inputActions.FindAction("ThrottleUp");
        //throttleDownAction = inputActions.FindAction("ThrottleDown");



        menuNextAction.performed += ctx => NextBtnPressed();
        moveAction.performed += ctx => HandleMovement(ctx);
        //lookAction.performed += ctx => HandleRotation(ctx);
        //throttleUpAction.performed += ctx => HandleThrottleUp(ctx);
        //throttleDownAction.performed += ctx => HandleThrottleDown(ctx);

        //controller = new GamepadControllers();

        // controller.Controller.MenuNext.performed += ctx => MenuNextBtn();

        //SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void Start()
    {
        if (IsControllerConnected())
        {
            Debug.Log("Game controller is connected!");
            DontDestroyOnLoad(gameObject);
            //droneMovementScript.SetJoystickCase();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        menuNextAction.Enable();

        // Check if the current scene is Main_Menu
        if (SceneManager.GetActiveScene().name != "Main_Menu")
        {
            EnableDroneInput();
        }
        else
        {
            DisableDroneInput();
        }

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the new scene is "Main_Menu"
        if (scene.name == "Main_Menu")
        {
            DisableDroneInput();
        }
        else
        {
            EnableDroneInput();
            StartCoroutine(FindDroneRacingClone());
        }
    }

    // Enable drone input actions (in all scenes except "Main_Menu")
    private void EnableDroneInput()
    {
        moveAction.Enable();
        //lookAction.Enable();
        //throttleUpAction.Enable();
        //throttleDownAction.Enable();
    }

    // Disable drone input actions (in the "Main_Menu" scene)
    private void DisableDroneInput()
    {
        moveAction.Disable();
        //lookAction.Disable();
        //throttleUpAction.Disable();
        //throttleDownAction.Disable();
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
            foreach (var item in Gamepad.all)
            {
                gamepad = item;
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

    private void HandleMovement(InputAction.CallbackContext context)
    {
        if (droneMovementScript == null)
        {
            Debug.LogError("DroneMovementScript is not assigned!");
            return; // Prevent further execution if the reference is null
        }

        Vector2 movementInput = context.ReadValue<Vector2>();
        // Translate this input into drone movement using WASD-like controls.
        // Movement on the left stick corresponds to W (forward), S (backward), A (left), D (right).
        float moveX = movementInput.x;
        float moveY = movementInput.y;

        // Apply movement logic to the drone (send this data to the drone's movement script)
        // You can pass this data to your drone movement script for translation to physics.
        Debug.Log($"Move - X: {moveX}, Y: {moveY}");

        // Use triggers for Z-axis movement (Up/Down)
        float moveZ = gamepad.leftTrigger.ReadValue() - gamepad.rightTrigger.ReadValue(); // L2 (left trigger) for up, R2 (right trigger) for down

        // Use right stick for rotation (yaw: left/right rotation)
        float rotationY = gamepad.rightStick.x.ReadValue(); // Right stick horizontal (rotation)

        droneMovementScript.SetMovement(moveX, moveY, moveZ, rotationY);

        // Pass movement data to the DroneMovementScript
/*        droneMovementScript.Horizontal_A = moveX < 0 ? 1 : 0; // A key (left)
        droneMovementScript.Horizontal_D = moveX > 0 ? 1 : 0; // D key (right)
        droneMovementScript.Vertical_W = moveY > 0 ? 1 : 0; // W key (forward)
        droneMovementScript.Vertical_S = moveY < 0 ? 1 : 0; // S key (backward)*/
    }

    /*    private void HandleMovement(InputAction.CallbackContext context)
        {
            if (droneMovementScript == null)
            {
                Debug.LogError("DroneMovementScript is not assigned!");
                return;
            }

            Vector2 movementInput = context.ReadValue<Vector2>();
            float moveX = movementInput.x;
            float moveY = movementInput.y;

            // Handle movement values and pass them to DroneMovement
            droneMovementScript.SetMovement(moveX, moveY);
        }*/

    private void HandleRotation(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();
        // Translate right stick input into drone rotation (yaw and pitch).
        float rotationX = lookInput.x;
        float rotationY = lookInput.y;

        // Apply rotation logic to the drone (send this data to the drone's rotation script)
        Debug.Log($"Look - Rotation X: {rotationX}, Y: {rotationY}");
    }

    private void HandleThrottleUp(InputAction.CallbackContext context)
    {
        // Logic for upward throttle (L2)
        Debug.Log("Throttle Up");
        // Send this input to the drone's upward movement script.
    }

    private void HandleThrottleDown(InputAction.CallbackContext context)
    {
        // Logic for downward throttle (R2)
        Debug.Log("Throttle Down");
        // Send this input to the drone's downward movement script.
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

    IEnumerator FindDroneRacingClone()
    {
        yield return new WaitUntil(() => GameObject.Find("Level_1(Clone)") != null);

        // Now find the GameObject and assign the DroneMovement script
        GameObject droneObject = GameObject.Find("Drone_Racing");
        if (droneObject != null)
        {
            droneMovementScript = droneObject.GetComponent<DroneMovement>();
            if (droneMovementScript != null)
            {
                Debug.Log("DroneMovement script successfully assigned.");
            }
            else
            {
                Debug.LogError("DroneMovement script not found on Drone_Racing(Clone).");
            }
        }
        else
        {
            Debug.LogError("Drone_Racing(Clone) GameObject not found in the scene.");
        }
    }
}
