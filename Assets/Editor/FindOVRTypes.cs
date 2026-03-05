using UnityEditor;
using UnityEngine;

public class FindOVRTypes : Editor
{
    [MenuItem("Tools/Find OVR Types Details")]
    public static void FindTypes()
    {
        foreach (System.Reflection.Assembly ass in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            var r = ass.GetType("OVRRaycaster");
            if (r != null) {
                Debug.Log("Found OVRRaycaster in assembly: " + ass.FullName);
                // Also search for Input modules in this assembly
                foreach(var t in ass.GetTypes()) {
                    if (t.Name.Contains("InputModule")) {
                        Debug.Log("Found InputModule: " + t.FullName);
                    }
                }
            }
        }
    }
}
