using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Wires three UI buttons to <see cref="DroneSelectionManager"/> and shows a "Selected" badge
/// on the active choice only. Position each badge top-right on its button in the editor (child of button).
/// </summary>
public class DroneSelectUI : MonoBehaviour
{
    [System.Serializable]
    public class DroneOptionUI
    {
        [Tooltip("Button for this drone (index matches DroneSelectionManager.availableDrones order).")]
        public Button button;

        [Tooltip("Shown only when this drone is selected. Place a small panel + Text anchored top-right on the button.")]
        public GameObject selectedBadge;

        [Tooltip("Optional: assign the Text on the badge to set label (e.g. \"Selected\").")]
        public Text badgeLabel;
    }

    [Header("References")]
    [Tooltip("Leave empty to use DroneSelectionManager.Instance at runtime.")]
    public DroneSelectionManager droneSelectionManager;

    [Tooltip("Exactly 3 entries for your three drone buttons.")]
    public DroneOptionUI[] options = new DroneOptionUI[3];

    [Header("Badge text")]
    public string selectedText = "Selected";

    [Header("When selection changes")]
    [Tooltip("If true, calling ApplySelectionInCurrentLevel on manager when already in a Level_* scene.")]
    public bool swapDroneImmediatelyInLevel = false;

    [Header("Close button")]
    [Tooltip("Panel root to hide when Close is pressed. Leave empty to disable the GameObject this script is on.")]
    public GameObject droneSelectPanel;

    private void OnEnable()
    {
        if (droneSelectionManager == null)
            droneSelectionManager = DroneSelectionManager.Instance;

        for (int i = 0; i < options.Length; i++)
        {
            int index = i;
            if (options[i].button != null)
                options[i].button.onClick.AddListener(() => OnDroneButtonClicked(index));
        }

        RefreshSelectedVisuals();
    }

    private void OnDisable()
    {
        for (int i = 0; i < options.Length; i++)
        {
            if (options[i].button != null)
                options[i].button.onClick.RemoveAllListeners();
        }
    }

    private void OnDroneButtonClicked(int droneIndex)
    {
        if (droneSelectionManager != null)
        {
            droneSelectionManager.SetSelectedDroneIndex(droneIndex);
            if (swapDroneImmediatelyInLevel && SceneManager.GetActiveScene().name.StartsWith("Level_"))
                droneSelectionManager.ApplySelectionInCurrentLevel();
        }
        else
        {
            PlayerPrefs.SetInt(DroneSelectionManager.PlayerPrefsSelectedDroneIndex, droneIndex);
            PlayerPrefs.Save();
        }

        RefreshSelectedVisuals();
    }

    /// <summary>Call after changing selection from code elsewhere.</summary>
    public void RefreshSelectedVisuals()
    {
        int current = 0;
        if (droneSelectionManager != null)
            current = droneSelectionManager.GetSelectedDroneIndex();
        else
            current = PlayerPrefs.GetInt(DroneSelectionManager.PlayerPrefsSelectedDroneIndex, 0);

        for (int i = 0; i < options.Length; i++)
        {
            bool isOn = i == current;
            if (options[i].selectedBadge != null)
                options[i].selectedBadge.SetActive(isOn);
            if (options[i].badgeLabel != null)
            {
                options[i].badgeLabel.text = selectedText;
                // Label visibility follows badge if both assigned; if only label, toggle gameObject
                if (options[i].selectedBadge == null)
                    options[i].badgeLabel.gameObject.SetActive(isOn);
            }
        }
    }

    private void Start()
    {
        RefreshSelectedVisuals();
    }

    /// <summary>
    /// Hook your Close button OnClick to this. Hides <see cref="droneSelectPanel"/> or this GameObject.
    /// </summary>
    public void CloseDroneSelectPanel()
    {
        if (droneSelectPanel != null)
            droneSelectPanel.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}
