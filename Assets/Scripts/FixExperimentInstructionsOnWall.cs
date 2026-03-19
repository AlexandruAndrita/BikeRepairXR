using UnityEngine;

[ExecuteAlways]
public class FixExperimentInstructionsOnWall : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    [SerializeField] private Vector3 localPosition = new Vector3(0f, 0f, 0.002f);
    [SerializeField] private Vector3 localEulerAngles = Vector3.zero;

    private void LateUpdate()
    {
        if (anchor == null) return;

        if (transform.parent != anchor)
            transform.SetParent(anchor, false);

        transform.localPosition = localPosition;
        transform.localRotation = Quaternion.Euler(localEulerAngles);
    }
}