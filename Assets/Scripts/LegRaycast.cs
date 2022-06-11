using UnityEngine;

public class LegRaycast : MonoBehaviour
{
    private RaycastHit hit;
    private Transform transform;

    public Vector3 Position => hit.point;

    public Vector3 Normal => hit.normal;

    private void Awake()
    {
        transform = base.transform;
    }

    void Update()
    {
        var ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit);
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        var transform = base.transform;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, hit.point);
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hit.point, 0.05f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(hit.point, hit.normal * 0.1f);
    }

#endif
}