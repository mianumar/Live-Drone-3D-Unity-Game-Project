using System.Collections;
using DroneController.Physics; // Ensure this namespace is correct
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // If still needed for UI elements

// It's good practice to put DroneMovementScript in its namespace if it isn't already
// using DroneController.Movement; // Assuming DroneMovement might be here

public class GameControllerManager : MonoBehaviour
{
    [Header("UI ELEMENTS")]
    public GameObject welcomePanel;
    // public GameObject TutorialPanel; // Reference might be in MainMenu now?
    public GameObject ScreenPanel;
    public Image[] nextImagesIcon; // For controller icon indication
    public Sprite SelectIcon;      // Icon to show when controller detected

    public AudioSource WELCOME_TO_DRONE_LIFE; // Audio for welcome sequence

    [Header("CONTROLLER SETTINGS")]
    public InputActionAsset inputActions; // Assign your Input Actions Asset here
    public MainMenu mainMenu; // Reference to MainMenu script for UI transitions
    public float findDroneTimeout = 10f; // Max seconds to wait for drone object

    // Actions
    private InputAction menuNextAction;
    private InputAction moveAction;       // Left Stick (Forward/Backward, Left/Right Strafe)
    private InputAction lookAction;       // Right Stick (Yaw/Rotation, potentially Pitch if needed later)
    private InputAction throttleUpAction; // e.g., Right Trigger
    private InputAction throttleDownAction;// e.g., Left Trigger
    private InputAction popupConfirmAction; // *** NEW: For Level End Popups ***

    // Drone Reference
    private DroneMovement droneMovementScript; // Changed to DroneMovement type
    private bool isDroneInputActive = false; // Flag to control input processing
    private Coroutine findDroneRoutine;

    // Panels Reference
    private const string LEVEL_COMPLETE_TAG = "LevelCompletePopup"; // Tag for Win Panel
    private const string DEAD_PANEL_TAG = "DeadPanelPopup";         // Tag for Lose Panel


    #region Unity Lifecycle Methods

    private void Awake()
    {
        if (IsControllerConnected())
        {
            Debug.Log("Gamepad connected. Persisting GameControllerManager.");
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene changes
        }
        else
        {
            Debug.LogWarning("No gamepad detected. Destroying GameControllerManager.");
            Destroy(gameObject);
            return; // Stop Awake execution if no controller
        }

        // --- Find Actions ---
        if (inputActions == null)
        {
            Debug.LogError("Input Actions Asset not assigned in the inspector!");
            Destroy(gameObject); // Can't function without actions
            return;
        }

        menuNextAction = inputActions.FindAction("MenuNext"); // Ensure action named "MenuNext" exists
        moveAction = inputActions.FindAction("moveAction");       // Ensure action named "Move" exists (Vector2, Left Stick)
        lookAction = inputActions.FindAction("lookAction");       // Ensure action named "Look" exists (Vector2, Right Stick)
        throttleUpAction = inputActions.FindAction("throttleUpAction"); // Ensure action named "ThrottleUp" exists (float, e.g., Right Trigger)
        throttleDownAction = inputActions.FindAction("throttleDownAction"); // Ensure action named "ThrottleDown" exists (float, e.g., Left Trigger)
        popupConfirmAction = inputActions.FindAction("PopupConfirm"); // to Manage Popup Inputs


        // --- Validate Actions ---
        if (menuNextAction == null || moveAction == null || lookAction == null || throttleUpAction == null || throttleDownAction == null || popupConfirmAction == null)
        {
            Debug.LogError("One or more required Input Actions could not be found! Check names in the Input Actions Asset ('MenuNext', 'Move', 'Look', 'ThrottleUp', 'ThrottleDown').");
            Destroy(gameObject);
            return;
        }

        // --- Subscribe to Discrete Actions ---
        // Only subscribe 'performed' for button-like actions
        menuNextAction.performed += ctx => HandleMenuNext();
        popupConfirmAction.performed += ctx2 => HandlePopupConfirm();

        Debug.Log("GameControllerManager Awake completed.");
    }

    void Start()
    {
        // Initial check for scene and input state
        UpdateInputStateForScene(SceneManager.GetActiveScene());
    }

    void Update()
    {
        // Continuously read analog/vector inputs if drone input should be active
        if (isDroneInputActive && droneMovementScript != null && droneMovementScript.customFeed)
        {
            ProcessDroneInputs();
        }

        // Optional: Add any other per-frame logic here (e.g., checking panel states if needed)
    }

    private void OnEnable()
    {
        // Enable actions when this component is enabled
        // Note: Scene-specific enabling/disabling is handled elsewhere
        menuNextAction?.Enable();
        moveAction?.Enable();
        lookAction?.Enable();
        throttleUpAction?.Enable();
        throttleDownAction?.Enable();
        popupConfirmAction?.Enable();
    }

    private void OnDisable()
    {
        // It's crucial to disable actions and unsubscribe from events to prevent errors
        menuNextAction?.Disable();
        moveAction?.Disable();
        lookAction?.Disable();
        throttleUpAction?.Disable();
        throttleDownAction?.Disable();
        popupConfirmAction?.Disable();

        // Unsubscribe from events
        if (menuNextAction != null) menuNextAction.performed -= ctx => HandleMenuNext();
        if (popupConfirmAction != null) popupConfirmAction.performed -= ctx2 => HandlePopupConfirm(); //Win and Lose Panel Handle
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from scene changes
    }

    private void OnDestroy()
    {
        // Additional cleanup just in case
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region Scene Management and Input State

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        UpdateInputStateForScene(scene);
    }

    private void UpdateInputStateForScene(Scene scene)
    {
        if (scene.name == "Main_Menu") // Name of your main menu scene
        {
            DisableDroneInputProcessing();
            // Optionally find MainMenu script reference if needed for this scene
            if (mainMenu == null) mainMenu = FindObjectOfType<MainMenu>();
        }
        else
        {
            // Attempt to find the drone and enable input processing
            if (findDroneRoutine != null)
                StopCoroutine(findDroneRoutine);
            findDroneRoutine = StartCoroutine(FindAndPrepareDrone());
        }
    }

    /// <summary>Call after <see cref="DroneSelectionManager"/> replaces the drone so gamepad input targets the new instance.</summary>
    public void RebindToSceneDroneAfterSwap()
    {
        if (inputActions == null)
            return;
        DisableDroneInputProcessing();
        droneMovementScript = null;
        if (findDroneRoutine != null)
            StopCoroutine(findDroneRoutine);
        findDroneRoutine = StartCoroutine(FindAndPrepareDrone());
    }

    IEnumerator FindAndPrepareDrone()
    {
        // Wait one frame so DroneSelectionManager can replace Drone_Racing after level env spawns.
        yield return null;

        Debug.Log("Attempting to find Drone...");
        droneMovementScript = null; // Reset reference
        float timeWaited = 0f;
        GameObject droneObject = null;

        // Try finding the drone object for a limited time
        while (droneObject == null && timeWaited < findDroneTimeout)
        {
            // --- Adjust the Find method as needed ---
            // Option 1: Find by specific name (if consistent)
            droneObject = GameObject.Find("Drone_Racing"); // Or "Drone_Racing(Clone)" if instantiated

            // Option 2: Find by Tag (Recommended if you can tag your drone prefab)
            // if (droneObject == null) {
            //    GameObject[] drones = GameObject.FindGameObjectsWithTag("PlayerDrone"); // Use a specific tag
            //    if (drones.Length > 0) droneObject = drones[0]; // Take the first one found
            // }

            // Option 3: After DroneSelectionManager swap, name stays Drone_Racing; fallback for edge cases
            if (droneObject == null)
            {
                DroneMovement foundScript = FindObjectOfType<DroneMovement>();
                if (foundScript != null)
                    droneObject = foundScript.gameObject;
            }
            // --- End Find method options ---

            if (droneObject == null)
            {
                yield return null; // Wait for the next frame
                timeWaited += Time.deltaTime;
            }
        }

        if (droneObject != null)
        {
            droneMovementScript = droneObject.GetComponent<DroneMovement>();
            if (droneMovementScript != null)
            {
                Debug.Log("DroneMovement script found and assigned.");
                EnableDroneInputProcessing();
                menuNextAction.Disable();
            }
            else
            {
                Debug.LogError($"DroneMovement script not found on GameObject '{droneObject.name}'. Disabling drone input.");
                DisableDroneInputProcessing();
            }
        }
        else
        {
            Debug.LogError($"Drone GameObject could not be found after {findDroneTimeout} seconds. Disabling drone input.");
            DisableDroneInputProcessing();
        }

        findDroneRoutine = null;
    }

    private void EnableDroneInputProcessing()
    {
        if (droneMovementScript == null)
        {
            Debug.LogError("Cannot enable drone input processing: DroneMovementScript reference is null.");
            return;
        }
        Debug.Log("Enabling Drone Input Processing.");
        isDroneInputActive = true;
        droneMovementScript.customFeed = true; // Crucial: Tell drone script to use our feed
        // Ensure base script input methods are potentially bypassed if customFeed is true
        // (The base script's SettingControllerToInputSettings should handle this)
    }

    private void DisableDroneInputProcessing()
    {
        Debug.Log("Disabling Drone Input Processing.");
        isDroneInputActive = false;
        if (droneMovementScript != null)
        {
            // Reset drone's input state if needed when disabling
            droneMovementScript.customFeed = false;
            droneMovementScript.customFeed_forward = 0f;
            droneMovementScript.customFeed_backward = 0f;
            droneMovementScript.customFeed_leftward = 0f;
            droneMovementScript.customFeed_rightward = 0f;
            droneMovementScript.customFeed_upward = 0f;
            droneMovementScript.customFeed_downward = 0f;
            droneMovementScript.customFeed_rotateLeft = 0f;
            droneMovementScript.customFeed_rotateRight = 0f;
        }
        // Keep droneMovementScript reference, might be needed if returning to game scene
    }

    #endregion

    #region Input Processing

    // Called from Update when drone input is active
    private void ProcessDroneInputs()
    {
        // --- Read current values from continuous actions ---
        Vector2 moveInput = moveAction.ReadValue<Vector2>();     // Left Stick XY
        Vector2 lookInput = lookAction.ReadValue<Vector2>();     // Right Stick XY
        float throttleUp = throttleUpAction.ReadValue<float>();   // Right Trigger (0 to 1)
        //Debug.Log("throttleUp " + throttleUp);
        float throttleDown = throttleDownAction.ReadValue<float>(); // Left Trigger (0 to 1)

        // --- Update DroneMovementScript's custom feed variables ---
        // Forward/Backward (Left Stick Y)
        droneMovementScript.customFeed_forward = Mathf.Max(0f, moveInput.y);
        droneMovementScript.customFeed_backward = Mathf.Max(0f, -moveInput.y);

        // Left/Right Strafe (Left Stick X)
        droneMovementScript.customFeed_rightward = Mathf.Max(0f, moveInput.x);
        droneMovementScript.customFeed_leftward = Mathf.Max(0f, -moveInput.x);

        // Rotation/Yaw (Right Stick X)
        droneMovementScript.customFeed_rotateRight = Mathf.Max(0f, lookInput.x);
        droneMovementScript.customFeed_rotateLeft = Mathf.Max(0f, -lookInput.x);

        // Up/Down Throttle (Triggers)
        droneMovementScript.customFeed_upward = throttleUp;
        droneMovementScript.customFeed_downward = throttleDown;

        // --- Optional: Pitch (Right Stick Y) ---
        // The base script doesn't seem to have a direct customFeed variable for pitch tilt.
        // If you wanted to control pitch tilt directly here, you would need to:
        // 1. Add a `customFeed_pitch` variable to DroneMovementScript.
        // 2. Modify `CustomInputFeed` in DroneMovementScript to use it.
        // 3. Modify `MovementForward` (or wherever tilt is applied) to potentially use this custom pitch value
        //    when `customFeed` is true, instead of calculating tilt based on W/S input.
        // Example line (IF you implement pitch feed):
        // droneMovementScript.customFeed_pitch = lookInput.y; // Value range -1 to 1
    }

    // Called when the MenuNext action is performed
    private void HandleMenuNext()
    {
        if(SceneManager.GetActiveScene().name != "Main_Menu")
        {
            menuNextAction?.Disable();
            return;
        }
        // This logic seems tied to specific panel states in the MainMenu scene
        Debug.Log("MenuNext Action Performed");

        if (mainMenu == null && SceneManager.GetActiveScene().name == "Main_Menu")
        {
            mainMenu = FindObjectOfType<MainMenu>(); // Try to find it if null
            if (mainMenu == null)
            {
                Debug.LogError("MainMenu script reference not found!");
                return;
            }
        }

        if (welcomePanel != null && welcomePanel.activeSelf)
        {
            //ShowTutorialPanel();
            Transform buttonTransform = welcomePanel.transform.Find("Button_Skip");

            if (buttonTransform != null) 
            {
                pressButton(buttonTransform);
                Debug.Log("Welcome Panel");
            }
        }
        else if (mainMenu != null && mainMenu.Tpanal != null && mainMenu.Tpanal.activeSelf)
        {
            //ShowVideoPanel();
            Transform buttonTransform = mainMenu.Tpanal.transform.Find("Button_Skip (1)");

            if (buttonTransform != null)
            {
                pressButton(buttonTransform);
                Debug.Log("T Panel");
            }
        }
        else if (ScreenPanel != null && ScreenPanel.activeSelf)
        {
            //ScreenPanelShow();
            Transform buttonTransform = ScreenPanel.transform.Find("Button_Start");

            if (buttonTransform != null)
            {
                pressButton(buttonTransform);
                Debug.Log("Screen Panel");
            }
        }
        else
        {
            Debug.Log("MenuNext pressed, but no known active panel state matched.");
        }
    }

    public void pressButton(Transform buttonTransform)
    {
        // Get the Button component
        UnityEngine.UI.Button button = buttonTransform.GetComponent<UnityEngine.UI.Button>();

        if (button != null)
        {
            // Simulate button press
            button.onClick.Invoke();
            Debug.Log("Button_Claim pressed successfully");
        }
        else
        {
            Debug.LogError("Button component not found on Button_Claim object");
        }
    }

    #endregion

    #region External Hooks for Popups (Call these from your popup activation scripts)

    public void HandlePopupConfirm()
    {
        Debug.Log("HandlePopupConfirm");
        // Get the levelCompletePanel GameObject
        GameObject levelCompletePanel = ScoreManager.instance.levelCompletePanel;
        GameObject deadPanel = ScoreManager.instance.deadPanel;

        if (levelCompletePanel != null && levelCompletePanel.activeSelf)
        {

            // Find the Result object first
            Transform resultTransform = levelCompletePanel.transform.Find("Result");

            if (resultTransform != null)
            {
                // Now find the Button_Claim under Result
                Transform buttonClaimTransform = resultTransform.Find("Button_Claim");

                if (buttonClaimTransform != null)
                {
                    // Get the Button component
                    UnityEngine.UI.Button buttonClaim = buttonClaimTransform.GetComponent<UnityEngine.UI.Button>();

                    if (buttonClaim != null)
                    {
                        // Simulate button press
                        buttonClaim.onClick.Invoke();
                        Debug.Log("Button_Claim pressed successfully");
                    }
                    else
                    {
                        Debug.LogError("Button component not found on Button_Claim object");
                    }
                }
                else
                {
                    Debug.LogError("Button_Claim not found under Result object");
                }
            }
            else
            {
                Debug.LogError("Result object not found under levelCompletePanel");
            }
        }
    else if (deadPanel != null && deadPanel.activeSelf)
        {

            // Find the Result object first
            Transform continueTransform = deadPanel.transform.Find("Continue");

            if (continueTransform != null)
            {
                // Now find the Button_Claim under Result
                Transform buttonContinueTransform = continueTransform.Find("Button_Continue");

                if (buttonContinueTransform != null)
                {
                    // Get the Button component
                    UnityEngine.UI.Button buttonClaim = buttonContinueTransform.GetComponent<UnityEngine.UI.Button>();

                    if (buttonClaim != null)
                    {
                        // Simulate button press
                        buttonClaim.onClick.Invoke();
                        Debug.Log("Button_Continue pressed successfully");
                    }
                    else
                    {
                        Debug.LogError("Button component not found on Continue object");
                    }
                }
                else
                {
                    Debug.LogError("Button_Continue not found under Continue object");
                }
            }
            else
            {
                Debug.LogError("continueTransform object not found under deadPanel");
            }

        }
    else
        {
            Debug.LogError("levelCompletePanel  and DeadPanel is null");
        }
    }

    #endregion

    #region UI Interaction Methods (Keep if needed, might belong in MainMenu)

    // These methods seem tightly coupled with the MainMenu script/scene.
    // Consider moving them to MainMenu.cs if they only operate on its elements.
    public void ShowTutorialPanel()
    {
        Debug.Log("Showing Tutorial Panel");
        if (welcomePanel != null) welcomePanel.SetActive(false);
        if (mainMenu != null && mainMenu.Tpanal != null) mainMenu.Tpanal.SetActive(true);
        // Enable audio source? Make sure it's assigned and valid.
        if (WELCOME_TO_DRONE_LIFE != null) WELCOME_TO_DRONE_LIFE.enabled = true; // Or Play() ?
    }

    public void ScreenPanelShow()
    {
        Debug.Log("Starting Game via ScreenPanelShow");
        if (mainMenu != null) mainMenu.startgame(); // Assuming this loads the game scene
    }

    public void ShowVideoPanel()
    {
        Debug.Log("Showing Video Panel");
        if (mainMenu != null) mainMenu.getStart(); // What does this do? Assumed part of UI flow
        if (ScreenPanel != null) ScreenPanel.SetActive(true);
    }

    #endregion

    #region Utility Methods

    // Function to check if a controller is connected
    bool IsControllerConnected()
    {
        if (Gamepad.current != null) // Check the currently active gamepad first
        {
            Debug.Log($"Active gamepad: {Gamepad.current.displayName}");
            UpdateNextButtonIcons(); // Update UI icons
            return true;
        }
        else if (Gamepad.all.Count > 0) // Check if any are connected, even if not active
        {
            Debug.Log($"Connected gamepads found ({Gamepad.all.Count}), but none active. Using first: {Gamepad.all[0].displayName}");
            UpdateNextButtonIcons(); // Update UI icons
            return true;
        }
        return false;
    }

    // Updates UI elements to show controller is detected (e.g., change "Press Enter" to "Press A")
    void UpdateNextButtonIcons()
    {
        if (nextImagesIcon == null || SelectIcon == null) return;

        foreach (var image in nextImagesIcon)
        {
            if (image != null) image.sprite = SelectIcon;
        }
    }

    #endregion
}