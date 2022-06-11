using UnityEngine;

[RequireComponent(typeof(BodyFollow))]
public class BodyTarget : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private LineRenderer line;
    private BodyFollow follow;

    private void Start()
    {
        follow = GetComponent<BodyFollow>();
    }
    
    private void Update()
    {
        follow.SetTarget(target.position, target.rotation);

        if (line != null)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, follow.TargetPosition);
        }
    }
}