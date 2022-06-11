using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BodyFollow : MonoBehaviour
{
    [Header("Linear")]
    [SerializeField]
    private float maxSpeed = 1;
    [SerializeField]
    private float acceleration = 1; 
    [Header("Angular")]
    [SerializeField]
    private float maxAngularSpeed = 1;
    [SerializeField]
    private float angularAcceleration = 1; 

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Rigidbody rigidbody;

    public Vector3 TargetPosition => targetPosition;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        SetTarget(transform.position, transform.rotation);
    }

    public void SetTarget(Vector3 position, Quaternion rotation)
    {
        targetPosition = position;
        targetRotation = rotation;
    } 

    private void FixedUpdate()
    {
        var fixedDeltaTime = Time.fixedDeltaTime;
        
        // Position
        var currentMaxSpeed = CalculateMaxVelocity(targetPosition - rigidbody.position, 
            maxSpeed, acceleration, fixedDeltaTime);
        rigidbody.velocity = Vector3.MoveTowards(rigidbody.velocity, currentMaxSpeed, 
            acceleration * fixedDeltaTime);
        
        //Rotation
        var deltaRotationQuaternion = targetRotation * Quaternion.Inverse(rigidbody.rotation);
        var deltaRotation = MinimizeAngles(deltaRotationQuaternion.eulerAngles) * Mathf.Deg2Rad;
        
        var currentMaxAngularSpeed = CalculateMaxVelocity(deltaRotation, 
            maxAngularSpeed, angularAcceleration, fixedDeltaTime);
        rigidbody.angularVelocity =  Vector3.MoveTowards(rigidbody.angularVelocity, 
            currentMaxAngularSpeed, angularAcceleration * fixedDeltaTime);
    }

    private static Vector3 CalculateMaxVelocity(in Vector3 deltaPosition, float maxSpeed, float acceleration, float deltaTime)
    {
        var deltaPositionLength = deltaPosition.magnitude;
        var currentMaxSpeed = SqrtMagnitude(deltaPosition * (acceleration * 2.0f));
        return Vector3.ClampMagnitude(currentMaxSpeed, Mathf.Min(maxSpeed, deltaPositionLength / deltaTime));
    }

    private static Vector3 SqrtMagnitude(Vector3 v)
    {
        var vMagnitude = v.magnitude;
        if (vMagnitude < float.Epsilon) return Vector3.zero;
        return Mathf.Sqrt(vMagnitude) * (v / vMagnitude);
    }

    private static Vector3 MinimizeAngles(Vector3 angles)
    {
        if (angles.x > 180) angles.x -= 360;
        else if (angles.x < -180) angles.x += 360;
        if (angles.y > 180) angles.y -= 360;
        else if (angles.y < -180) angles.y += 360;
        if (angles.z > 180) angles.z -= 360;
        else if (angles.z < -180) angles.z += 360;
        return angles;
    }
}
