using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject welcomePanel;
    public GameObject descriptionPanel;
    
    [Header("Buttons")]
    public Button continueButton;
    public Button startButton;
    
    [Header("Scene Settings")]
    public string scene1Name = "BicycleScene";
    
void Start()
{
    // Auto-find UI elements if not assigned
    if (welcomePanel == null)
    {
        welcomePanel = GameObject.Find("WelcomePanel");
    }
    if (descriptionPanel == null)
    {
        descriptionPanel = GameObject.Find("DescriptionPanel");
    }
    if (continueButton == null)
    {
        GameObject continueBtnObj = GameObject.Find("ContinueButton");
        if (continueBtnObj != null) continueButton = continueBtnObj.GetComponent<Button>();
    }
    if (startButton == null)
    {
        GameObject startBtnObj = GameObject.Find("StartButton");
        if (startBtnObj != null) startButton = startBtnObj.GetComponent<Button>();
    }
    
    // Show welcome panel, hide description panel
    if (welcomePanel != null) welcomePanel.SetActive(true);
    if (descriptionPanel != null) descriptionPanel.SetActive(false);
    
    // Add listeners to buttons
    if (continueButton != null)
    {
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(OnContinueClicked);
    }
    else
    {
        Debug.LogError("ContinueButton not found!");
    }
    
    if (startButton != null)
    {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(OnStartClicked);
    }
    else
    {
        Debug.LogError("StartButton not found!");
    }
    
    Debug.Log("IntroUIManager initialized successfully");
}
    
void OnContinueClicked()
{
    Debug.Log("Continue button clicked!");
    if (welcomePanel != null) welcomePanel.SetActive(false);
    if (descriptionPanel != null) descriptionPanel.SetActive(true);
}
    
void OnStartClicked()
{
    Debug.Log("Start button clicked! Loading scene: " + scene1Name);
    SceneManager.LoadScene(scene1Name);
}
    
    void OnDestroy()
    {
        continueButton.onClick.RemoveListener(OnContinueClicked);
        startButton.onClick.RemoveListener(OnStartClicked);
    }


void OnGUI()
{
    if (welcomePanel == null || descriptionPanel == null)
    {
        GUI.Label(new Rect(10, 10, 400, 30), "ERROR: UI Panels not found!");
    }
    if (continueButton == null)
    {
        GUI.Label(new Rect(10, 40, 400, 30), "ERROR: Continue Button not found!");
    }
    if (startButton == null)
    {
        GUI.Label(new Rect(10, 70, 400, 30), "ERROR: Start Button not found!");
    }
}
}