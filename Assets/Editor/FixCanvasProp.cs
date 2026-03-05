using UnityEditor;
using UnityEngine;

public class FixCanvasProp : Editor
{
    [MenuItem("Tools/Fix Canvas Property")]
    public static void Apply()
    {
        // 1. Assign _canvas to PointableCanvas
        GameObject canvasObj = GameObject.Find("IntroCanvas");
        if (canvasObj != null)
        {
            var pCanvasTypes = canvasObj.GetComponents<Component>();
            foreach (var comp in pCanvasTypes)
            {
                if (comp.GetType().Name == "PointableCanvas")
                {
                    SerializedObject so = new SerializedObject(comp);
                    so.FindProperty("_canvas").objectReferenceValue = canvasObj.GetComponent<Canvas>();
                    so.ApplyModifiedProperties();
                }
            }
            
            // Re-add GraphicRaycaster if needed
            if (canvasObj.GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
            {
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
        }

        // 2. Clear old input modules and add PointableCanvasModule
        GameObject eSys = GameObject.Find("EventSystem");
        if (eSys != null)
        {
            var p1 = eSys.GetComponent("OVRInputModule");
            if (p1 != null) DestroyImmediate(p1);
            var p2 = eSys.GetComponent("InputSystemUIInputModule");
            if (p2 != null) DestroyImmediate(p2);
            var p3 = eSys.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            if (p3 != null) DestroyImmediate(p3);

            System.Type ptModuleType = null;
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                ptModuleType = asm.GetType("Oculus.Interaction.PointableCanvasModule");
                if (ptModuleType != null) break;
            }

            if (ptModuleType != null)
            {
                if (eSys.GetComponent(ptModuleType) == null)
                {
                    eSys.AddComponent(ptModuleType);
                }
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("Canvas properties fixed.");
    }
}
