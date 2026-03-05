using UnityEngine;
using UnityEngine.UI;

public class IntroFlowController : MonoBehaviour
{
    public GameObject welcomePanel;
    public GameObject descriptionPanel;

    public Button startButton;
    public Button backButton;
    public Button nextButton;
    public int currentPage = 0;

    void Start()
    {
        if (welcomePanel != null) welcomePanel.SetActive(true);
        if (descriptionPanel != null) descriptionPanel.SetActive(false);

        if (startButton != null) startButton.onClick.AddListener(OnStartPressed);
        if (nextButton != null) nextButton.onClick.AddListener(OnNextPressed);
        if (backButton != null) backButton.onClick.AddListener(OnBackPressed);
    }

    void OnStartPressed()
    {
        if (welcomePanel != null) welcomePanel.SetActive(false);
        if (descriptionPanel != null) descriptionPanel.SetActive(true);
    }

    void OnNextPressed()
    {
        Debug.Log("Next button pressed! Action applies later.");
        currentPage++;
    }
    void OnBackPressed()
    {
        Debug.Log("Back button pressed! Action applies later.");
        currentPage--;
        if (currentPage == 0)
        {
            if (welcomePanel != null) welcomePanel.SetActive(true);
            if (descriptionPanel != null) descriptionPanel.SetActive(false);
        }
    }
}