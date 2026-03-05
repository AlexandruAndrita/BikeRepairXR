using UnityEngine;
using UnityEditor;

public class FixIntroSceneGeneric {
    [MenuItem("Tools/Fix Intro Scene Generic")]
    public static void Fix() {
        bool changed = false;
        
        var canvasObj = GameObject.Find("BillboardCanvas");
        if (canvasObj != null) {
            var canvas = canvasObj.GetComponent<Canvas>();
            if (canvas != null) {
                canvas.renderMode = RenderMode.WorldSpace;
            }

            var rt = canvasObj.GetComponent<RectTransform>();
            if (rt != null) {
                rt.SetParent(null);
                rt.position = new Vector3(0, 1.5f, 2f);
                rt.rotation = Quaternion.identity;
                rt.localScale = new Vector3(0.002f, 0.002f, 0.002f);
            }

            var eyeAnchor = GameObject.Find("CenterEyeAnchor");
            if (eyeAnchor != null) {
                var cam = eyeAnchor.GetComponent<Camera>();
                if (canvas != null && cam != null) {
                    canvas.worldCamera = cam;
                }
            }
            
            var raycaster = canvasObj.GetComponent("OVRRaycaster");
            if (raycaster != null) {
                var so = new SerializedObject(raycaster);
                var prop = so.FindProperty("pointer");
                if (prop != null) {
                    var rightAnchor = GameObject.Find("RightControllerAnchor");
                    if (rightAnchor != null) {
                        prop.objectReferenceValue = rightAnchor;
                        so.ApplyModifiedProperties();
                    }
                }
            }
            EditorUtility.SetDirty(canvasObj);
            changed = true;
        }
        
        var eventSystem = GameObject.Find("EventSystem");
        if (eventSystem != null) {
            var inputModule = eventSystem.GetComponent("OVRInputModule");
            if (inputModule != null) {
                var so = new SerializedObject(inputModule);
                var prop = so.FindProperty("rayTransform");
                if (prop != null) {
                    var rightAnchor = GameObject.Find("RightControllerAnchor");
                    if (rightAnchor != null) {
                        prop.objectReferenceValue = rightAnchor.transform;
                        so.ApplyModifiedProperties();
                    }
                }
            }
            EditorUtility.SetDirty(eventSystem);
            changed = true;
        }
        
        if (changed) {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
            Debug.Log("Scene fixed successfully!");
        } else {
            Debug.LogError("Could not find BillboardCanvas or EventSystem.");
        }
    }
}
