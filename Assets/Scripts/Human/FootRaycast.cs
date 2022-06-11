using UnityEngine;

public class FootRaycast : MonoBehaviour
{
    [SerializeField] 
    private Rigidbody body;
    [SerializeField] 
    private float originOffset = 0.5f;
    [SerializeField] 
    private float overshoot = 0.1f;
    [SerializeField] 
    private float groundOffset = 0.06f;

    private Vector3? fixedPosition;

    void Update()
    {
        var bodyUp = body.rotation * Vector3.up;
        var legPosition = transform.position;
        var ray = new Ray(legPosition + bodyUp * originOffset, -bodyUp);

        if (Physics.Raycast(ray, out var hit, originOffset + overshoot))
        {
            var currentFootHeight = transform.localPosition.y;
            var targetPosition = hit.point + bodyUp * currentFootHeight;
            if (fixedPosition == null)
            {
                if (currentFootHeight < groundOffset)
                {
                    fixedPosition = targetPosition;
                }
            }
            else if (currentFootHeight > groundOffset)
            {
                fixedPosition = null;
            }
            else
            {
                var velocity = (fixedPosition.Value - transform.position) / Time.deltaTime;
                velocity = Vector3.ProjectOnPlane(velocity, bodyUp);
                body.velocity = velocity;
                targetPosition = fixedPosition.Value;
            }

            transform.position = targetPosition;
        }
        else
        {
            fixedPosition = null;
        }
    }
}