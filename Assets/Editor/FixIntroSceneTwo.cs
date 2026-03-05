using UnityEditor;
using UnityEngine;

public class FixIntroSceneTwo : Editor
{
    [MenuItem("Tools/Fix Intro Scene Two")]
    public static void FixIntroTwo()
    {
        GameObject[] roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject root in roots)
        {
            if (root.name.Contains("Low-Poly Bicycle"))
            {
                DestroyImmediate(root);
            }
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
}
