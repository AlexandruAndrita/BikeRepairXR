using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class SetupBrightLighting
{
    [MenuItem("Tools/Fix OVR Rig Position")]
    public static void FixOVRPosition()
    {
        var ovrRig = GameObject.Find("OVRCameraRig");
        if (ovrRig == null) { Debug.LogWarning("OVRCameraRig not found!"); return; }

        // Place the rig on the garage floor, centered, facing the workbench wall
        ovrRig.transform.position = new Vector3(0f, 0f, 1.5f);
        ovrRig.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        EditorUtility.SetDirty(ovrRig);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log($"OVRCameraRig moved to (0, 0, 1.5) facing 180° (toward workbench).");
    }

    [MenuItem("Tools/Log Scene Positions")]
    public static void LogPositions()
    {
        var objects = new[] { "OVRCameraRig", "Garage", "Low-Poly Bicycle # 5", "Low-Poly Bicycle # 5 (1)" };
        foreach (var objName in objects)
        {
            var go = GameObject.Find(objName);
            if (go != null)
                Debug.Log($"[{objName}] pos={go.transform.position}, rot={go.transform.eulerAngles}");
            else
                Debug.Log($"[{objName}] NOT FOUND");
        }
    }

    [MenuItem("Tools/Apply Bright Lighting")]
    public static void Apply()
    {
        // Configure the Directional Light
        var lightGO = GameObject.Find("Directional Light");
        if (lightGO != null)
        {
            var light = lightGO.GetComponent<Light>();
            if (light != null)
            {
                light.intensity = 1.5f;
                light.color = new Color(1f, 0.98f, 0.92f);
                light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                light.shadows = LightShadows.Soft;
                EditorUtility.SetDirty(lightGO);
            }
        }

        // Boost ambient lighting (Gradient/Trilight mode for bright interior)
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor    = new Color(0.86f, 0.90f, 0.95f);
        RenderSettings.ambientEquatorColor = new Color(0.75f, 0.78f, 0.82f);
        RenderSettings.ambientGroundColor  = new Color(0.45f, 0.42f, 0.38f);
        RenderSettings.ambientIntensity = 1.5f;

        // Boost Reflection Probe to match bright reference
        var rp = Object.FindObjectOfType<ReflectionProbe>();
        if (rp != null)
        {
            rp.intensity = 1.0f;
            EditorUtility.SetDirty(rp.gameObject);
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("Bright lighting applied successfully!");
    }
}
