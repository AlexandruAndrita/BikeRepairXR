using UnityEngine;
using UnityEditor;

public class FixCanvasVR {
    [MenuItem("Tools/Fix Canvas VR")]
    public static void Fix() {
        var canvas = GameObject.Find("BillboardCanvas").GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        var rt = canvas.GetComponent<RectTransform>();
        rt.SetParent(null); // Ensure it is a root object
        
        // Put it exactly at 1.5m height and 2m forward from origin
        rt.position = new Vector3(0, 1.5f, 2f);
        rt.rotation = Quaternion.identity;
        rt.localScale = new Vector3(0.002f, 0.002f, 0.002f);
        
        // Ensure no other scripts exist that could move it
        var comps = canvas.gameObject.GetComponents<Component>();
        foreach (var c in comps) {
            if (!(c is RectTransform) && !(c is Canvas) && !(c is UnityEngine.UI.CanvasScaler) && !(c is CanvasRenderer) && !(c.GetType().Name.Contains("Raycaster"))) {
                Debug.LogWarning("Removing unknown component: " + c.GetType().Name);
                Object.DestroyImmediate(c);
            }
        }
        
        EditorUtility.SetDirty(canvas.gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log("Canvas fixed: forced WorldSpace, position applied, and separated from anything else.");
    }
}
