using UnityEngine;
using TMPro;

public class NameInputUI : MonoBehaviour
{
    public static NameInputUI Instance;

    public GameObject panel;
    public TMP_InputField inputField;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnSubmit()
    {
        string name = inputField.text.Trim();

        if (name.Length < 2) return;

        PlayerData.Instance.SaveName(name);

        panel.SetActive(false);
        Time.timeScale = 1f;

        // 👉 CONTINUE FLOW (go to tutorial)
        FindObjectOfType<GameControllerManager>().ShowTutorialPanel();
    }
}