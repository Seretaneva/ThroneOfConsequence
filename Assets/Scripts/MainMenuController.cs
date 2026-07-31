using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "SampleScene";

    [Header("Main Menu")]
    [SerializeField] private GameObject buttonsPanel;
    [SerializeField] private Button continueButton;

    [Header("Optional Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Optional Feedback")]
    [SerializeField] private TMP_Text statusText;

    private void Awake()
    {
        FindMissingReferences();
        DisableTextRaycasts();
        ClosePanels();

        if (continueButton != null)
            continueButton.interactable = SaveManager.HasSave;

        SetStatus("");
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            ClosePanels();
    }

    public void StartNewGame()
    {
        Debug.Log("MainMenu: New Game pressed. Loading " + gameSceneName);
        SaveManager.DeleteSave();
        GameFlags.ClearFlags();

        if (!Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            SetStatus("Scena jocului nu este adaugata in Build Profiles.");
            Debug.LogError("MainMenu: scene not found in Build Profiles: " + gameSceneName);
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        if (!SaveManager.PrepareContinue())
        {
            SetStatus("Nu exista o salvare valida.");
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        ShowOnlyPanel(settingsPanel, "Panoul Settings nu este legat in Inspector.");
    }

    public void OpenCredits()
    {
        ShowOnlyPanel(creditsPanel, "Panoul Credits nu este legat in Inspector.");
    }

    public void ClosePanels()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (buttonsPanel != null)
            buttonsPanel.SetActive(true);

        SetStatus("");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ShowOnlyPanel(GameObject panel, string missingPanelMessage)
    {
        if (panel == null)
        {
            SetStatus(missingPanelMessage);
            Debug.LogWarning("MainMenu: " + missingPanelMessage);
            return;
        }

        if (settingsPanel != null)
            settingsPanel.SetActive(panel == settingsPanel);

        if (creditsPanel != null)
            creditsPanel.SetActive(panel == creditsPanel);

        if (buttonsPanel != null)
            buttonsPanel.SetActive(false);

        panel.SetActive(true);
        SetStatus("");
    }

    private void FindMissingReferences()
    {
        if (buttonsPanel == null)
            buttonsPanel = GameObject.Find("ButtonsPanel");

        if (settingsPanel == null)
            settingsPanel = GameObject.Find("SettingsPanel");

        if (creditsPanel == null)
            creditsPanel = GameObject.Find("CreditsPanel");

        if (continueButton == null)
        {
            GameObject continueObject = GameObject.Find("ContinueButton");

            if (continueObject != null)
                continueButton = continueObject.GetComponent<Button>();
        }
    }

    private void DisableTextRaycasts()
    {
        // Text labels are decorative. If they receive raycasts (or accidentally
        // contain a Button component), they can intercept clicks from the real button.
        TMP_Text[] labels = FindObjectsByType<TMP_Text>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (TMP_Text label in labels)
            label.raycastTarget = false;
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }
}
