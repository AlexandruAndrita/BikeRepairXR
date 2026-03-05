using UnityEngine;

public class OrbitAroundTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float orbitSpeed = 50f;
    [SerializeField] private float orbitRadius = 3f;
    [SerializeField] private float height = 2f;
    
    private float angle = 0f;

    void Update()
    {
        if (target == null) return;
        
        angle += orbitSpeed * Time.deltaTime;
        
        float x = target.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * orbitRadius;
        float z = target.position.z + Mathf.Sin(angle * Mathf.Deg2Rad) * orbitRadius;
        float y = target.position.y + height;
        
        transform.position = new Vector3(x, y, z);
        transform.Rotate(Vector3.up, orbitSpeed * Time.deltaTime);
    }
}
