using UnityEngine;
using UnityEngine.InputSystem;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;

    private Camera mainCamera;
    private Rigidbody rigidbody;
    private Vector2 mousePosition;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
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

            // Rotation
            rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation,
                Quaternion.LookRotation(directionToTarget, up),
                rotationSpeed * Time.fixedDeltaTime);
        }
    }
}