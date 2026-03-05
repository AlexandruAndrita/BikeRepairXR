using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Enables both OVR controllers to interact with a WorldSpace UI Canvas.
/// Point either controller at a button and pull the index trigger to click it.
/// The script auto-resolves the OVRCameraRig controller anchors and the Canvas
/// in the scene, so no manual wiring is required unless you want to override.
/// 
/// Attach this component to any active GameObject in the scene (e.g. IntroCanvas).
/// </summary>
[AddComponentMenu("XR/Controller UI Interactor")]
public class ControllerUIInteractor : MonoBehaviour
{
    [Header("Target Canvas (auto-found if blank)")]
    public Canvas targetCanvas;

    [Header("Max ray distance (metres)")]
    public float maxRayDistance = 10f;

    [Header("Laser line renderers (auto-created if blank)")]
    public LineRenderer leftLaser;
    public LineRenderer rightLaser;

    private Transform _leftController;
    private Transform _rightController;

    private void Start()
    {
        // Resolve controller anchors from the OVRCameraRig in the scene
        OVRCameraRig rig = FindFirstObjectByType<OVRCameraRig>();
        if (rig != null)
        {
            _leftController  = rig.leftControllerAnchor;
            _rightController = rig.rightControllerAnchor;
        }
        else
        {
            Debug.LogWarning("[ControllerUIInteractor] OVRCameraRig not found in scene. " +
                             "Controller UI interaction will not work.");
        }

        // Find the target canvas if not already assigned
        if (targetCanvas == null)
            targetCanvas = FindFirstObjectByType<Canvas>();

        if (targetCanvas == null)
            Debug.LogWarning("[ControllerUIInteractor] No Canvas found in scene.");

        // Create simple laser-pointer line renderers if none were provided
        if (leftLaser == null)
            leftLaser  = CreateLaser("LeftUILaser");
        if (rightLaser == null)
            rightLaser = CreateLaser("RightUILaser");
    }

    private void Update()
    {
        bool leftTrigger  = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger,
                                              OVRInput.Controller.LTouch);
        bool rightTrigger = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger,
                                              OVRInput.Controller.RTouch);

        ProcessController(_leftController,  leftTrigger,  leftLaser);
        ProcessController(_rightController, rightTrigger, rightLaser);
    }

    // -------------------------------------------------------------------------

    private void ProcessController(Transform anchor, bool triggered, LineRenderer laser)
    {
        if (anchor == null || targetCanvas == null) return;

        Ray ray = new Ray(anchor.position, anchor.forward);

        // Build the canvas plane — normal points outward (toward the viewer)
        Plane canvasPlane = new Plane(targetCanvas.transform.forward,
                                      targetCanvas.transform.position);

        float dist;
        bool hitCanvas = canvasPlane.Raycast(ray, out dist) && dist <= maxRayDistance;
        Vector3 endpoint = hitCanvas ? ray.GetPoint(dist) : ray.GetPoint(maxRayDistance);

        // Draw the laser from anchor to hit/endpoint
        if (laser != null)
        {
            laser.enabled = true;
            laser.SetPosition(0, anchor.position);
            laser.SetPosition(1, endpoint);
        }

        if (!hitCanvas || !triggered) return;

        // Find the first active, interactable button whose rect contains the hit point
        foreach (Button btn in targetCanvas.GetComponentsInChildren<Button>())
        {
            if (!btn.IsInteractable()) continue;

            if (WorldPointInRect(btn.GetComponent<RectTransform>(), endpoint))
            {
                btn.onClick.Invoke();
                return;
            }
        }
    }

    /// <summary>Returns true if <paramref name="worldPoint"/> lies inside the
    /// RectTransform's rectangle (in its local XY plane).</summary>
    private static bool WorldPointInRect(RectTransform rt, Vector3 worldPoint)
    {
        Vector3 local = rt.InverseTransformPoint(worldPoint);
        return rt.rect.Contains(new Vector2(local.x, local.y));
    }

    private static LineRenderer CreateLaser(string goName)
    {
        var go = new GameObject(goName);
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startWidth    = 0.004f;
        lr.endWidth      = 0.001f;
        lr.useWorldSpace = true;

        // Use the built-in Sprites/Default shader for a simple coloured line
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = Color.cyan;
        lr.material    = mat;
        lr.startColor  = new Color(0f, 1f, 1f, 0.9f);
        lr.endColor    = new Color(0f, 1f, 1f, 0.0f);
        return lr;
    }
}
