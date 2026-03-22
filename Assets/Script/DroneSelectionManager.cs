using DroneController.CameraMovement;
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

        // Level_* scripts Instantiate the env prefab in Start(), which runs before sceneLoaded fires.
        TrySwapDroneNow();
    }

    /// <summary>Replace placeholder with selected prefab. Safe to call from UI for in-level change.</summary>
    public void TrySwapDroneNow()
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
