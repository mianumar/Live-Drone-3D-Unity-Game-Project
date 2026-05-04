using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NameInputUI : MonoBehaviour
{
    public static NameInputUI Instance;

    public GameObject panel;
    public TMP_InputField inputField;
    public Button submitButton;

    void Awake()
    {
        Instance = this;

        panel.SetActive(false);

        // 🔥 Listen to input changes
        inputField.onValueChanged.AddListener(OnNameChanged);

        // Initially disable button
        submitButton.interactable = false;
    }

    public void Show()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;

        inputField.text = "";
        submitButton.interactable = false;
    }

    void OnNameChanged(string value)
    {
        // Trim spaces and validate
        string trimmed = value.Trim();

        // Enable only if valid
        submitButton.interactable = trimmed.Length >= 3 && !trimmed.Contains(" ");
    }

    public void OnSubmit()
    {
        string name = inputField.text.Trim();

        if (name.Length < 2) return;

        PlayerData.Instance.SaveName(name);

        panel.SetActive(false);
        Time.timeScale = 1f;

        FindObjectOfType<GameControllerManager>().ShowTutorialPanel();
    }
}