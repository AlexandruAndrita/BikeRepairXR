using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FixIntroCanvasInteractable : Editor
{
    static System.Type FindType(string name)
    {
        foreach (System.Reflection.Assembly ass in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            System.Type t = ass.GetType(name);
            if (t != null) return t;
        }
        return null;
    }

    [MenuItem("Tools/Rebuild Intro Canvas Interactable")]
    public static void Rebuild()
    {
        // 1. Destroy old canvas if any
        GameObject oldCanvas = GameObject.Find("IntroCanvas");
        if (oldCanvas != null) DestroyImmediate(oldCanvas);

        // 2. Setup Canvas
        GameObject canvasObj = new GameObject("IntroCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        RectTransform canvasRt = canvasObj.GetComponent<RectTransform>();
        canvasRt.position = new Vector3(0, 1.5f, -0.3f);
        canvasRt.rotation = Quaternion.Euler(0, 90f, 0); // Face camera
        canvasRt.sizeDelta = new Vector2(800, 600);
        canvasRt.localScale = new Vector3(0.002f, 0.002f, 0.002f);

        canvasObj.AddComponent<CanvasScaler>();

        // Add correct raycaster for Oculus Interaction if available
        System.Type ovrRaycasterType = FindType("OVRRaycaster");
        if (ovrRaycasterType != null)
        {
            var rc = canvasObj.AddComponent(ovrRaycasterType);
            SerializedObject so = new SerializedObject(rc);
            SerializedProperty pointerProp = so.FindProperty("pointer");
            if (pointerProp != null)
            {
                var rController = GameObject.Find("RightControllerAnchor");
                if (rController != null) {
                    pointerProp.objectReferenceValue = rController;
                    so.ApplyModifiedProperties();
                }
            }
        }
        else
        {
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Add correct event system module
        GameObject eventSystem = GameObject.Find("EventSystem");
        if (eventSystem != null)
        {
            System.Type ovrInputModuleType = FindType("OVRInputModule");
            if (ovrInputModuleType != null)
            {
                var inputModule = eventSystem.GetComponent("OVRInputModule") ?? eventSystem.AddComponent(ovrInputModuleType);
                SerializedObject so = new SerializedObject(inputModule);
                SerializedProperty rayTransformProp = so.FindProperty("rayTransform");
                if (rayTransformProp != null)
                {
                    var rController = GameObject.Find("RightControllerAnchor");
                    if (rController != null) {
                        rayTransformProp.objectReferenceValue = rController.transform;
                        so.ApplyModifiedProperties();
                    }
                }
            }
        }

        // 3. Welcome Panel
        GameObject welcomePanel = new GameObject("WelcomePanel");
        welcomePanel.transform.SetParent(canvasObj.transform, false);
        Image p1Image = welcomePanel.AddComponent<Image>();
        p1Image.color = new Color(0, 0, 0, 0.8f);
        RectTransform p1Rt = welcomePanel.GetComponent<RectTransform>();
        p1Rt.anchorMin = Vector2.zero; p1Rt.anchorMax = Vector2.one;
        p1Rt.sizeDelta = Vector2.zero; p1Rt.anchoredPosition = Vector2.zero;

        GameObject welcomeTextObj = new GameObject("WelcomeText");
        welcomeTextObj.transform.SetParent(welcomePanel.transform, false);
        TextMeshProUGUI tmpWelcome = welcomeTextObj.AddComponent<TextMeshProUGUI>();
        tmpWelcome.text = "Welcome to our experiment.";
        tmpWelcome.fontSize = 50; tmpWelcome.alignment = TextAlignmentOptions.Center;
        tmpWelcome.color = Color.white;
        RectTransform wTextRt = tmpWelcome.GetComponent<RectTransform>();
        wTextRt.sizeDelta = new Vector2(700, 200); wTextRt.anchoredPosition = new Vector2(0, 100);

        GameObject startBtnObj = DefaultControls.CreateButton(new DefaultControls.Resources());
        startBtnObj.name = "StartButton";
        startBtnObj.transform.SetParent(welcomePanel.transform, false);
        RectTransform startBtnRt = startBtnObj.GetComponent<RectTransform>();
        startBtnRt.sizeDelta = new Vector2(250, 80); startBtnRt.anchoredPosition = new Vector2(0, -100);
        DestroyImmediate(startBtnObj.GetComponentInChildren<Text>().gameObject);
        GameObject startBtnTmpObj = new GameObject("Text (TMP)");
        startBtnTmpObj.transform.SetParent(startBtnObj.transform, false);
        TextMeshProUGUI startBtnTmp = startBtnTmpObj.AddComponent<TextMeshProUGUI>();
        startBtnTmp.text = "Start"; startBtnTmp.fontSize = 36; startBtnTmp.color = Color.black; startBtnTmp.alignment = TextAlignmentOptions.Center;
        RectTransform startBtnTmpRt = startBtnTmpObj.GetComponent<RectTransform>();
        startBtnTmpRt.anchorMin = Vector2.zero; startBtnTmpRt.anchorMax = Vector2.one;
        startBtnTmpRt.sizeDelta = Vector2.zero; startBtnTmpRt.anchoredPosition = Vector2.zero;

        // 4. Description Panel
        GameObject descPanel = new GameObject("DescriptionPanel");
        descPanel.transform.SetParent(canvasObj.transform, false);
        Image p2Image = descPanel.AddComponent<Image>();
        p2Image.color = new Color(0, 0, 0, 0.8f);
        RectTransform p2Rt = descPanel.GetComponent<RectTransform>();
        p2Rt.anchorMin = Vector2.zero; p2Rt.anchorMax = Vector2.one;
        p2Rt.sizeDelta = Vector2.zero; p2Rt.anchoredPosition = Vector2.zero;

        GameObject descTextObj = new GameObject("DescriptionText");
        descTextObj.transform.SetParent(descPanel.transform, false);
        TextMeshProUGUI tmpDesc = descTextObj.AddComponent<TextMeshProUGUI>();
        tmpDesc.text = "In this Experiment the player will need to perform 3 tasks on a virtual bicycle.";
        tmpDesc.fontSize = 36; tmpDesc.alignment = TextAlignmentOptions.Center;
        tmpDesc.color = Color.white;
        RectTransform dTextRt = tmpDesc.GetComponent<RectTransform>();
        dTextRt.sizeDelta = new Vector2(700, 400); dTextRt.anchoredPosition = new Vector2(0, 50);

        GameObject nextBtnObj = DefaultControls.CreateButton(new DefaultControls.Resources());
        nextBtnObj.name = "NextButton";
        nextBtnObj.transform.SetParent(descPanel.transform, false);
        RectTransform nextBtnRt = nextBtnObj.GetComponent<RectTransform>();
        nextBtnRt.sizeDelta = new Vector2(200, 60);
        // Anchor and Pivot bottom right
        nextBtnRt.anchorMin = new Vector2(1, 0); nextBtnRt.anchorMax = new Vector2(1, 0);
        nextBtnRt.pivot = new Vector2(1, 0);
        nextBtnRt.anchoredPosition = new Vector2(-20, 20);
        DestroyImmediate(nextBtnObj.GetComponentInChildren<Text>().gameObject);
        GameObject nextBtnTmpObj = new GameObject("Text (TMP)");
        nextBtnTmpObj.transform.SetParent(nextBtnObj.transform, false);
        TextMeshProUGUI nextBtnTmp = nextBtnTmpObj.AddComponent<TextMeshProUGUI>();
        nextBtnTmp.text = "Next"; nextBtnTmp.fontSize = 30; nextBtnTmp.color = Color.black; nextBtnTmp.alignment = TextAlignmentOptions.Center;
        RectTransform nextBtnTmpRt = nextBtnTmpObj.GetComponent<RectTransform>();
        nextBtnTmpRt.anchorMin = Vector2.zero; nextBtnTmpRt.anchorMax = Vector2.one;
        nextBtnTmpRt.sizeDelta = Vector2.zero; nextBtnTmpRt.anchoredPosition = Vector2.zero;

        // 5. Attach Simple Flow Controller Script
        IntroFlowController flow = canvasObj.AddComponent<IntroFlowController>();
        flow.welcomePanel = welcomePanel;
        flow.descriptionPanel = descPanel;
        flow.startButton = startBtnObj.GetComponent<Button>();
        flow.nextButton = nextBtnObj.GetComponent<Button>();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("IntroCanvas Interactable Rebuilt successfully.");
    }
}
