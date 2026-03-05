using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class ApplyInteractionSDKCanvas : Editor
{
    [MenuItem("Tools/Apply Interaction SDK Canvas")]
    public static void Apply()
    {
        // 1. Clean Controller Laser
        GameObject rightController = GameObject.Find("RightControllerAnchor");
        if (rightController != null)
        {
            var slp = rightController.GetComponent("SimpleLaserPointer");
            if (slp != null) DestroyImmediate(slp);
            var lr = rightController.GetComponent<LineRenderer>();
            if (lr != null) DestroyImmediate(lr);
        }

        // 2. Setup Canvas
        GameObject canvasObj = GameObject.Find("IntroCanvas");
        if (canvasObj != null)
        {
            // Remove old rigid OVRRaycaster
            var ovrRaycaster = canvasObj.GetComponent("OVRRaycaster");
            if (ovrRaycaster != null) DestroyImmediate(ovrRaycaster);
            
            // Re-add GraphicRaycaster if missing
            var gr = canvasObj.GetComponent<UnityEngine.UI.GraphicRaycaster>();
            if (gr == null) canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // Find PointableCanvas type
            System.Type ptCanvasType = null;
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                ptCanvasType = asm.GetType("Oculus.Interaction.PointableCanvas");
                if (ptCanvasType != null) break;
            }

            if (ptCanvasType != null)
            {
                var ptCanvas = canvasObj.GetComponent(ptCanvasType);
                if (ptCanvas == null)
                {
                    ptCanvas = canvasObj.AddComponent(ptCanvasType);
                    SerializedObject so = new SerializedObject(ptCanvas);
                    so.FindProperty("_canvas").objectReferenceValue = canvasObj.GetComponent<Canvas>();
                    so.ApplyModifiedProperties();
                }
            }
            else
            {
                Debug.LogError("Oculus.Interaction.PointableCanvas not found!");
            }
        }

        // 3. Setup Event System
        GameObject eventSystem = GameObject.Find("EventSystem");
        if (eventSystem != null)
        {
            // Clean old modules
            var inSys = eventSystem.GetComponent("InputSystemUIInputModule");
            if (inSys != null) DestroyImmediate(inSys);
            var stdInSys = eventSystem.GetComponent<StandaloneInputModule>();
            if (stdInSys != null) DestroyImmediate(stdInSys);
            var ovrInSys = eventSystem.GetComponent("OVRInputModule");
            if (ovrInSys != null) DestroyImmediate(ovrInSys);

            // Add PointableCanvasModule
            System.Type ptModuleType = null;
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                ptModuleType = asm.GetType("Oculus.Interaction.PointableCanvasModule");
                if (ptModuleType != null) break;
            }

            if (ptModuleType != null)
            {
                var ptModule = eventSystem.GetComponent(ptModuleType);
                if (ptModule == null)
                {
                    eventSystem.AddComponent(ptModuleType);
                }
            }
            else
            {
                Debug.LogError("Oculus.Interaction.PointableCanvasModule not found!");
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("Completed setting up Interaction SDK Canvas logic.");
    }
}
