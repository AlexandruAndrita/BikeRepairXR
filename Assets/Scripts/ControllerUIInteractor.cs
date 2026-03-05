using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fallback OVR controller → WorldSpace UI button interaction.
/// Casts a ray from each controller anchor; on index trigger press it invokes
/// the first Button whose rect the ray intersects.
///
/// No laser is drawn here — the ISDK ray visual (from ComprehensiveInteraction)
/// is the sole visual. Once a BoxCollider + RayInteractable are added to the
/// canvas in the Unity Editor and wired to the PointableCanvas component, this
/// script can be removed and the ISDK system handles everything natively.
/// </summary>
[AddComponentMenu("XR/Controller UI Interactor")]
public class ControllerUIInteractor : MonoBehaviour
{
    [Header("Target Canvas (auto-found if blank)")]
    public Canvas targetCanvas;

    [Header("Max ray distance (metres)")]
    public float maxRayDistance = 10f;

    private Transform _leftController;
    private Transform _rightController;

    private void Start()
    {
        OVRCameraRig rig = FindFirstObjectByType<OVRCameraRig>();
        if (rig != null)
        {
            _leftController  = rig.leftControllerAnchor;
            _rightController = rig.rightControllerAnchor;
        }
        else
        {
            Debug.LogWarning("[ControllerUIInteractor] OVRCameraRig not found in scene.");
        }

        if (targetCanvas == null)
            targetCanvas = FindFirstObjectByType<Canvas>();

        if (targetCanvas == null)
            Debug.LogWarning("[ControllerUIInteractor] No Canvas found in scene.");
    }

    private void Update()
    {
        bool leftTrigger  = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger,
                                              OVRInput.Controller.LTouch);
        bool rightTrigger = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger,
                                              OVRInput.Controller.RTouch);

        if (leftTrigger)  TryClickButton(_leftController);
        if (rightTrigger) TryClickButton(_rightController);
    }

    private void TryClickButton(Transform anchor)
    {
        if (anchor == null || targetCanvas == null) return;

        Ray ray = new Ray(anchor.position, anchor.forward);
        Plane canvasPlane = new Plane(targetCanvas.transform.forward,
                                      targetCanvas.transform.position);

        float dist;
        if (!canvasPlane.Raycast(ray, out dist) || dist > maxRayDistance) return;

        Vector3 hitPoint = ray.GetPoint(dist);

        foreach (Button btn in targetCanvas.GetComponentsInChildren<Button>())
        {
            if (!btn.IsInteractable()) continue;

            if (WorldPointInRect(btn.GetComponent<RectTransform>(), hitPoint))
            {
                btn.onClick.Invoke();
                return;
            }
        }
    }

    private static bool WorldPointInRect(RectTransform rt, Vector3 worldPoint)
    {
        Vector3 local = rt.InverseTransformPoint(worldPoint);
        return rt.rect.Contains(new Vector2(local.x, local.y));
    }
}
