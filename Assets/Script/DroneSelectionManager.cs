using DroneController.CameraMovement;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central place to choose which drone prefab is used in levels.
/// Level prefabs (Resources/Level_*) contain a placeholder named <see cref="placeholderObjectName"/> (default: Drone_Racing).
/// After the level scene loads (and Level_* Start has instantiated the env), this replaces that object with the selected prefab
/// and re-wires ScoreManager, LastLevelScoe, and DroneCamera.
/// </summary>
[DefaultExecutionOrder(-50)]
public class DroneSelectionManager : MonoBehaviour
{
    public const string PlayerPrefsSelectedDroneIndex = "SelectedDroneIndex";

    public static DroneSelectionManager Instance { get; private set; }

    [Tooltip("Must match the GameObject name in your Level_* prefabs under Resources.")]
    public string placeholderObjectName = "Drone_Racing";

    [Tooltip("Assign your drone prefabs here (same order as UI / indices). Index 0 should match your default placeholder drone.")]
    public DronePrefabEntry[] availableDrones;

    [System.Serializable]
    public class DronePrefabEntry
    {
        public string displayName;
        public GameObject prefab;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!scene.name.StartsWith("Level_"))
            return;

        // Use a coroutine to wait for the Level script to finish instantiating the environment
        StartCoroutine(WaitAndSwap());
    }

    private IEnumerator WaitAndSwap()
    {
        // Wait for two frames to ensure the 'Level_1(Clone)' is fully settled in the hierarchy
        yield return null;
        yield return new WaitForEndOfFrame();

        TrySwapDroneNow();
    }

    /// <summary>Replace placeholder with selected prefab. Safe to call from UI for in-level change.</summary>
    /*    public void TrySwapDroneNow()
        {
            var placeholder = GameObject.Find(placeholderObjectName);
            if (placeholder == null)
            {
                Debug.LogWarning($"DroneSelectionManager: No '{placeholderObjectName}' found in scene.");
                return;
            }

            if (availableDrones == null || availableDrones.Length == 0)
                return;

            int index = Mathf.Clamp(PlayerPrefs.GetInt(PlayerPrefsSelectedDroneIndex, 0), 0, availableDrones.Length - 1);
            var entry = availableDrones[index];
            if (entry == null || entry.prefab == null)
            {
                Debug.LogWarning($"DroneSelectionManager: No prefab at index {index}.");
                return;
            }

            var t = placeholder.transform;
            Transform parent = t.parent;
            Vector3 pos = t.position;
            Quaternion rot = t.rotation;
            Vector3 scale = t.localScale;

            Destroy(placeholder);

            var newDrone = Instantiate(entry.prefab, pos, rot, parent);
            newDrone.name = placeholderObjectName;
            newDrone.tag = "Player";
            newDrone.transform.localScale = scale;

            RewireSceneToDrone(newDrone);
            var gcm = Object.FindObjectOfType<GameControllerManager>();
            if (gcm != null)
                gcm.RebindToSceneDroneAfterSwap();
            Debug.Log($"DroneSelectionManager: Swapped to '{entry.displayName}' (index {index}).");
        }
    */
    /*
        public void TrySwapDroneNow()
        {
            // 1. Find the placeholder even if it's nested deep inside 'Level_1'
            GameObject placeholder = null;

            // This finds the object even if it's inactive or a nested child
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                // We check for the name and ensure it's part of the current scene (not a prefab asset)
                if (obj.name == placeholderObjectName && obj.hideFlags == HideFlags.None)
                {
                    placeholder = obj;
                    break;
                }
            }

            if (placeholder == null)
            {
                // Fallback: search for clones if Unity renamed it during instantiation
                placeholder = GameObject.Find(placeholderObjectName + "(Clone)");
            }

            if (placeholder == null)
            {
                Debug.LogWarning($"DroneSelectionManager: Could not find '{placeholderObjectName}' in the hierarchy.");
                return;
            }

            // 2. Capture the exact transform data from the child object
            Transform t = placeholder.transform;
            Vector3 pos = t.position;
            Quaternion rot = t.rotation;
            Transform parent = t.parent; // This keeps it inside the 'Level_1' group
            Vector3 scale = t.localScale;

            // 3. Destroy the old one and spawn your selection
            Destroy(placeholder);

            int index = GetSelectedDroneIndex();
            GameObject newDrone = Instantiate(availableDrones[index].prefab, pos, rot, parent);

            // 4. Clean up naming so other scripts (like GameController) can find it
            newDrone.name = placeholderObjectName;
            newDrone.transform.localScale = scale;

            // 5. Re-wire the game systems
            RewireSceneToDrone(newDrone);

            Debug.Log($"Successfully replaced nested {placeholderObjectName} with selection index {index}");
        }
    */

    public void TrySwapDroneNow()
    {
        // 1. Aggressive Search: Find the placeholder even if it's nested or part of a (Clone)
        GameObject placeholder = null;

        // We use FindObjectsOfTypeAll to find the placeholder even if it's deep in the hierarchy
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            // Ensure we only pick the object in the current scene, not the prefab in the Project folder
            if (obj.name == placeholderObjectName && obj.scene.isLoaded)
            {
                placeholder = obj;
                break;
            }
        }

        // Fallback search in case Unity renamed the object during instantiation
        if (placeholder == null)
        {
            placeholder = GameObject.Find(placeholderObjectName + "(Clone)");
        }

        if (placeholder == null)
        {
            Debug.LogWarning($"DroneSelectionManager: Still could not find '{placeholderObjectName}' in the hierarchy.");
            return;
        }

        if (availableDrones == null || availableDrones.Length == 0)
        {
            Debug.LogError("DroneSelectionManager: No availableDrones prefabs assigned!");
            return;
        }

        // 2. Get the selection index from PlayerPrefs (Saved in the Menu)
        int index = Mathf.Clamp(PlayerPrefs.GetInt(PlayerPrefsSelectedDroneIndex, 0), 0, availableDrones.Length - 1);
        var entry = availableDrones[index];

        if (entry == null || entry.prefab == null)
        {
            Debug.LogWarning($"DroneSelectionManager: No prefab assigned at index {index}.");
            return;
        }

        // 3. Capture the placeholder's Transform data
        Transform t = placeholder.transform;
        Vector3 pos = t.position;
        Quaternion rot = t.rotation;
        Transform parent = t.parent; // This keeps the new drone inside the Level_1(Clone) parent
        Vector3 scale = t.localScale;

        // 4. Destroy the placeholder and Instantiate the chosen drone
        Destroy(placeholder);

        var newDrone = Instantiate(entry.prefab, pos, rot, parent);

        // 5. Setup the new drone instance
        newDrone.name = placeholderObjectName; // Reset name so other scripts can find it easily
        newDrone.tag = "Player";
        newDrone.transform.localScale = scale;

        // 6. Re-wire the Scene systems to the new drone
        RewireSceneToDrone(newDrone);

        // 7. Re-bind Gamepad Input so it targets the new instance
        var gcm = Object.FindObjectOfType<GameControllerManager>();
        if (gcm != null)
        {
            gcm.RebindToSceneDroneAfterSwap();
        }

        Debug.Log($"DroneSelectionManager: Swapped to '{entry.displayName}' (index {index}) at {pos}.");
    }


    private static void RewireSceneToDrone(GameObject drone)
    {
        var movement = drone.GetComponent<DroneMovement>();
        var propellers = drone.GetComponent<DronePropelers>();

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.droneMovement = movement;
            ScoreManager.instance.DronePropelers = propellers;
        }

        var lastLevel = Object.FindObjectOfType<LastLevelScoe>();
        if (lastLevel != null)
        {
            lastLevel.droneMovement = movement;
            lastLevel.DronePropelers = propellers;
        }

        var droneCam = Object.FindObjectOfType<DroneCamera>();
        if (droneCam != null)
        {
            droneCam.ourDrone = drone;
            droneCam.pickedMyDrone = true;
        }
    }

    /// <summary>UI: set which drone to use on next level load (and optionally re-swap in current level).</summary>
    public void SetSelectedDroneIndex(int index)
    {
        if (availableDrones == null || availableDrones.Length == 0)
            return;
        index = Mathf.Clamp(index, 0, availableDrones.Length - 1);
        PlayerPrefs.SetInt(PlayerPrefsSelectedDroneIndex, index);
        PlayerPrefs.Save();
    }

    public int GetSelectedDroneIndex()
    {
        if (availableDrones == null || availableDrones.Length == 0)
            return 0;
        return Mathf.Clamp(PlayerPrefs.GetInt(PlayerPrefsSelectedDroneIndex, 0), 0, availableDrones.Length - 1);
    }

    /// <summary>Call from in-game UI to swap drone without reloading the scene.</summary>
    public void ApplySelectionInCurrentLevel()
    {
        if (!SceneManager.GetActiveScene().name.StartsWith("Level_"))
            return;
        TrySwapDroneNow();
    }
}
