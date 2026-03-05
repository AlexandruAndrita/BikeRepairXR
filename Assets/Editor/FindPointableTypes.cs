using UnityEditor;
using UnityEngine;
using System.Linq;

public class FindPointableTypes : Editor
{
    [MenuItem("Tools/Find Interaction Types")]
    public static void FindTypes()
    {
        foreach (System.Reflection.Assembly asm in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            var types = asm.GetTypes();
            foreach(var t in types) {
                if (t.Name.Contains("PointableCanvas") || t.Name.Contains("CanvasCylinder") || t.Name.Contains("RayInteractable")) {
                    Debug.Log("Found Interaction SDK type: " + t.FullName);
                }
            }
        }
    }
}
