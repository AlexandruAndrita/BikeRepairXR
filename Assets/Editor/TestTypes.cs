using UnityEditor;
using UnityEngine;

public class TestTypes : Editor
{
    [MenuItem("Tools/Test OVR Types")]
    public static void Test()
    {
        Debug.Log("OVRInputModule exists: " + (System.Type.GetType("OVRInputModule, Assembly-CSharp") != null));
        Debug.Log("OVRRaycaster exists: " + (System.Type.GetType("OVRRaycaster, Assembly-CSharp") != null));
    }
}
