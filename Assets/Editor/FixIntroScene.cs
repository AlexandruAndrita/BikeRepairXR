using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FixIntroScene : Editor
{
    [MenuItem("Tools/Fix Intro Scene")]
    public static void FixIntro()
    {
        // 1. Delete bicycles
        GameObject bike1 = GameObject.Find("Low-Poly Bicycle # 5");
        if (bike1 != null) DestroyImmediate(bike1);

        GameObject bike2 = GameObject.Find("Low-Poly Bicycle # 5 (1)");
        if (bike2 != null) DestroyImmediate(bike2);

        // 2. Create WorldSpace Canvas
        GameObject canvasObj = new GameObject("IntroCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        RectTransform canvasRt = canvasObj.GetComponent<RectTransform>();
        canvasRt.position = new Vector3(0, 1.5f, -0.3f);
        canvasRt.rotation = Quaternion.Euler(0, 90f, 0); // Face the camera at X ~ 3.26
        canvasRt.sizeDelta = new Vector2(800, 600);
        canvasRt.localScale = new Vector3(0.002f, 0.002f, 0.002f);

        // Optional: Add a panel/background
        GameObject panelObj = new GameObject("Background");
        panelObj.transform.SetParent(canvasObj.transform, false);
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);
        RectTransform panelRt = panelObj.GetComponent<RectTransform>();
        panelRt.anchorMin = Vector2.zero;
        panelRt.anchorMax = Vector2.one;
        panelRt.sizeDelta = Vector2.zero;
        panelRt.anchoredPosition = Vector2.zero;

        // 3. Create Welcome Text
        GameObject textObj = new GameObject("WelcomeText");
        textObj.transform.SetParent(canvasObj.transform, false);
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = "Welcome to our experiment.";
        tmpText.fontSize = 50;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.white;
        RectTransform textRt = tmpText.GetComponent<RectTransform>();
        textRt.sizeDelta = new Vector2(700, 200);
        textRt.anchoredPosition = new Vector2(0, 100);

        // 4. Create Start Button
        // Using Unity's Default Controls to properly setup a button
        GameObject buttonObj = DefaultControls.CreateButton(new DefaultControls.Resources());
        buttonObj.name = "StartButton";
        buttonObj.transform.SetParent(canvasObj.transform, false);
        RectTransform buttonRt = buttonObj.GetComponent<RectTransform>();
        buttonRt.sizeDelta = new Vector2(250, 80);
        buttonRt.anchoredPosition = new Vector2(0, -100);
        
        // Find existing text in button and replace or use TMP
        Text oldText = buttonObj.GetComponentInChildren<Text>();
        if (oldText != null) DestroyImmediate(oldText.gameObject);

        GameObject btnTextObj = new GameObject("Text (TMP)");
        btnTextObj.transform.SetParent(buttonObj.transform, false);
        TextMeshProUGUI btnTmp = btnTextObj.AddComponent<TextMeshProUGUI>();
        btnTmp.text = "Start";
        btnTmp.fontSize = 36;
        btnTmp.color = Color.black;
        btnTmp.alignment = TextAlignmentOptions.Center;
        RectTransform btnTextRt = btnTmp.GetComponent<RectTransform>();
        btnTextRt.anchorMin = Vector2.zero;
        btnTextRt.anchorMax = Vector2.one;
        btnTextRt.sizeDelta = Vector2.zero;
        btnTextRt.anchoredPosition = Vector2.zero;

        // Mark scene as dirty
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        Debug.Log("IntroScene UI created successfully.");
    }
}
