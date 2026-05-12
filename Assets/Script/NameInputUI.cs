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

        CheckAndShowPopup();

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

    private void CheckAndShowPopup()
    {
        string savedName = PlayerPrefs.GetString("UserName", "");

        if (string.IsNullOrEmpty(savedName))
        {
            Show();
        }
        else
        {
            panel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    void OnNameChanged(string value)
    {
        // Trim spaces and validate
        string trimmed = value.Trim();

        // Validation: length check and no spaces
        submitButton.interactable = trimmed.Length >= 3 && !trimmed.Contains(" ");
    }

    public void OnSubmit()
    {
        string name = inputField.text.Trim();

        if (name.Length < 3) return;

        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.SaveName(name);

            if (SQLHighScoreManager.Instance != null)
            {
                SQLHighScoreManager.Instance.UpdateHighScore(0);
            }
        }

        panel.SetActive(false);
        Time.timeScale = 1f;

        //FindObjectOfType<GameControllerManager>().ShowTutorialPanel();
    }
}