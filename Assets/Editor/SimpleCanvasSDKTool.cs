using UnityEditor;
using UnityEngine;

public class SimpleCanvasSDKTool : Editor
{
    [MenuItem("Tools/Re-apply Pointable Canvas")]
    public static void Apply()
    {
        GameObject cObj = GameObject.Find("IntroCanvas");
        if (cObj == null) {
            Debug.LogError("IntroCanvas not found");
            return;
        }

        var oldR = cObj.GetComponent("OVRRaycaster");
        if (oldR != null) DestroyImmediate(oldR);
        
        // Oculus.Interaction.PointableCanvas
        System.Type pCanvasType = FindType("PointableCanvas");
        if (pCanvasType != null) {
            if (cObj.GetComponent(pCanvasType) == null) {
                var c = cObj.AddComponent(pCanvasType);
                var so = new SerializedObject(c);
                var cp = so.FindProperty("_canvas");
                if (cp != null) cp.objectReferenceValue = cObj.GetComponent<Canvas>();
                so.ApplyModifiedProperties();
                Debug.Log("Added PointableCanvas");
            }
        } else {
            Debug.LogError("PointableCanvas not found by type name");
        }

        GameObject eSys = GameObject.Find("EventSystem");
        if (eSys != null) {
            var inputSys = eSys.GetComponent("InputSystemUIInputModule");
            if (inputSys != null) DestroyImmediate(inputSys);
            var stdSys = eSys.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            if (stdSys != null) DestroyImmediate(stdSys);
            var ovrSys = eSys.GetComponent("OVRInputModule");
            if (ovrSys != null) DestroyImmediate(ovrSys);
            
            System.Type pModuleType = FindType("PointableCanvasModule");
            if (pModuleType != null) {
                if (eSys.GetComponent(pModuleType) == null) {
                    eSys.AddComponent(pModuleType);
                    Debug.Log("Added PointableCanvasModule");
                }
            } else {
                Debug.LogError("PointableCanvasModule not found");
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }

    static System.Type FindType(string name) {
        foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies()) {
            foreach(var t in asm.GetTypes()) {
                if (t.Name == name) return t;
            }
        }
        return null;
    }
}
