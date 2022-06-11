using UnityEngine;

public class BodyRaycast : MonoBehaviour
{
    [SerializeField]
    private float raycastOffset = 1;

    [SerializeField] 
    private float distance;

    [SerializeField]
    private float moveSpeed = 1;

    private Transform transform;
    private Rigidbody rigidbody;

    void Start()
    {
        transform = base.transform;
        rigidbody = GetComponent<Rigidbody>();
    }
        
    private void FixedUpdate()
    {
        var transformUp = transform.up;
        var ray = new Ray(transform.position + raycastOffset * transformUp, -transformUp);
        if (Physics.Raycast(ray, out var hit, raycastOffset + distance))
        {
            rigidbody.position = Vector3.Lerp(
                rigidbody.position, 
                hit.point + transformUp * distance,
                moveSpeed * Time.deltaTime);
            var rigidbodyVelocity = rigidbody.velocity;
            rigidbodyVelocity.y = 0;
            rigidbody.velocity = rigidbodyVelocity;
        }
    }
}