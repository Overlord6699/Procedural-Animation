using UnityEngine;
using UnityEngine.InputSystem;

public class MechControl : MonoBehaviour
{
    [SerializeField]
    private float distanceFromGround = 0.5f;
    [Header("Movement")]
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float movementAcceleration;
    [Header("Rotation")]
    [SerializeField]
    private float rotationSpeed;

    private Camera mainCamera;
    private Rigidbody rigidbody;
    private InputAction moveAction;
    private float movementInput;
    private Vector2 mousePosition;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        moveAction = new InputAction("Move", InputActionType.Button, "Mouse/rightButton");
        moveAction.Enable();
    }

    private void OnDestroy()
    {
        moveAction.Dispose();
    }

    private void Update()
    {
        movementInput = moveAction.ReadValue<float>();
        mousePosition = Mouse.current.position.ReadValue();
    }

    private void FixedUpdate()
    {
        var cameraRay = mainCamera.ScreenPointToRay(mousePosition);
        var up = rigidbody.rotation * Vector3.up;
        var rigidbodyPosition = rigidbody.position;
        var mechPlane = new Plane(up, rigidbodyPosition);
        if (mechPlane.Raycast(cameraRay, out var e))
        {
            var targetPoint = cameraRay.GetPoint(e);
            var directionToTarget = targetPoint - rigidbodyPosition;

            // Position
            var movementSpeed = directionToTarget.normalized * this.movementSpeed * movementInput;
            
            var rigidbodyVelocity = rigidbody.velocity;
            rigidbodyVelocity = Vector3.MoveTowards(
                rigidbodyVelocity, 
                movementSpeed,
                movementAcceleration * Time.fixedDeltaTime);
            rigidbody.velocity = rigidbodyVelocity;

            if (rigidbodyPosition.y < distanceFromGround)
            {
                rigidbody.position = new Vector3(rigidbodyPosition.x, distanceFromGround, rigidbodyPosition.z);
                rigidbody.velocity = new Vector3(rigidbodyVelocity.x, 
                    Mathf.Max(0, rigidbodyVelocity.y), rigidbodyVelocity.z);
            }
            
            // Rotation
            rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, 
                Quaternion.LookRotation(directionToTarget, up), 
                rotationSpeed * Time.fixedDeltaTime);
        }
        
    }
}