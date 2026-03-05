using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebugUIManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string scene1Name = "Scene 1";
    
    private bool showWelcome = true;
    private bool showDescription = false;
    private GUIStyle titleStyle;
    private GUIStyle textStyle;
    private GUIStyle buttonStyle;
    
    void Start()
    {
        // Initialize styles
        titleStyle = new GUIStyle();
        titleStyle.fontSize = 48;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.normal.textColor = Color.white;
        
        textStyle = new GUIStyle();
        textStyle.fontSize = 32;
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.normal.textColor = Color.white;
        textStyle.wordWrap = true;
        
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 36;
        buttonStyle.alignment = TextAnchor.MiddleCenter;
        buttonStyle.normal.textColor = Color.white;
        buttonStyle.normal.background = MakeTex(2, 2, new Color(0.2f, 0.5f, 0.8f));
        buttonStyle.hover.background = MakeTex(2, 2, new Color(0.3f, 0.6f, 0.9f));
    }
    
    void OnGUI()
    {
        float centerX = Screen.width / 2;
        float centerY = Screen.height / 2;
        
        if (showWelcome)
        {
            DrawWelcomeScreen(centerX, centerY);
        }
        else if (showDescription)
        {
            DrawDescriptionScreen(centerX, centerY);
        }
    }
    
    void DrawWelcomeScreen(float centerX, float centerY)
    {
        // Semi-transparent background
        GUI.color = new Color(0, 0, 0, 0.8f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
        
        // Title
        GUI.Label(new Rect(centerX - 400, centerY - 150, 800, 100), "Welcome to Our Experiment", titleStyle);
        
        // Continue Button
        if (GUI.Button(new Rect(centerX - 150, centerY + 50, 300, 80), "Continue", buttonStyle))
        {
            showWelcome = false;
            showDescription = true;
            Debug.Log("Continue button clicked!");
        }
    }
    
    void DrawDescriptionScreen(float centerX, float centerY)
    {
        // Semi-transparent background
        GUI.color = new Color(0, 0, 0, 0.8f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
        
        // Title
        GUI.Label(new Rect(centerX - 400, centerY - 250, 800, 100), "Experiment Description", titleStyle);
        
        // Description text
        string description = "This experiment will test your ability to perform tasks in a virtual environment. " +
                           "Please follow the instructions carefully. " +
                           "The experiment consists of multiple scenes where you will interact with virtual objects.";
        
        GUI.Label(new Rect(centerX - 400, centerY - 100, 800, 200), description, textStyle);
        
        // Start Button
        if (GUI.Button(new Rect(centerX - 150, centerY + 150, 300, 80), "Start Experiment", buttonStyle))
        {
            Debug.Log("Start button clicked! Loading scene: " + scene1Name);
            SceneManager.LoadScene(scene1Name);
        }
    }
    
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}