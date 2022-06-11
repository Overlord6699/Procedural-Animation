using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Spider : MonoBehaviour
{
    [Header("Legs")]
    [SerializeField] 
    private LegData[] legs;
    
    [SerializeField]
    private Transform raycastsPlane;

    [SerializeField] 
    private float speed = 10;

    [SerializeField]
    private float stepLength = 0.5f;
    [SerializeField]
    private float stepHeight = 0.1f;

    [SerializeField]
    private float raycastsOffset = 1f;
    
    [SerializeField]
    private float heightAboveGround = 0.75f;

    [SerializeField]
    private Transform movementTarget;
    [SerializeField]
    private Transform mouseTarget;

    [SerializeField]
    private float Lean = 1;

    private Rigidbody rigidbody;
    private InputActions inputActions;

    private Vector3 inputMovement;
    private float inputRotation;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        inputActions = new InputActions();
        inputActions.Enable();

        for (var index = 0; index < legs.Length; index++)
        {
            var raycastOrigin = legs[index].RaycastOrigin;
            var ray = new Ray(raycastOrigin.position, raycastOrigin.forward);
            if (Physics.Raycast(ray, out var hit))
            {
                legs[index].target2.Position = hit.point;
            }
        }

        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }

    private void FixedUpdate()
    {
        //Debug.Log(inputMovement);


        // var rigidbodyPosition = rigidbody.position;
        // var rigidbodyVelocity = rigidbody.velocity;
        //
        // var directionToFloor = Vector3.Project(GroundPlanePosition - rigidbodyPosition, -GroundPlaneNormal);
        // var projectionOnFloor = rigidbodyPosition + directionToFloor;
        //
        //
        // //rigidbody.position = projectionOnFloor + GroundPlaneNormal * heightAboveGround;
        // var targetPosition = projectionOnFloor + GroundPlaneNormal * heightAboveGround;
        //
        // var rotation = Quaternion.LookRotation(rigidbody.rotation * Vector3.forward, GroundPlaneNormal);
        //
        // //rigidbody.position += transform.rotation * (inputMovement * (Time.deltaTime * movementSpeed));
        // //rigidbody.rotation = rotation * Quaternion.Euler(0, inputRotation * rotationSpeed * Time.deltaTime, 0);
        // targetPosition += rigidbody.rotation * (inputMovement * (Time.fixedDeltaTime * movementSpeed));
        //
        // var drag = rigidbodyVelocity * -dragAcceleration;
        // // rigidbody.velocity = Vector3.MoveTowards(
        // //     rigidbodyVelocity, 
        // //     rigidbody.rotation * inputMovement * movementSpeed, Time.deltaTime * bodyAcceleration
        // //     );
        //
        // rigidbody.AddForce((targetPosition - rigidbodyPosition).normalized * bodyAcceleration + drag, ForceMode.Acceleration);

    }

    void Update()
    {
        var legCount = legs.Length;
        var averagePosition = Vector3.zero;
        var averageNormal = Vector3.zero;
        for (var index = 0; index < legs.Length; index++)
        {
            var raycastOrigin = legs[index].RaycastOrigin;
            var ray = new Ray(raycastOrigin.position, raycastOrigin.forward);
            if (Physics.Raycast(ray, out var hit))
            {
                var currentStepLength = stepLength;
                if (legs[index].TargetPosition != null)
                {
                    // In Movement
                    legs[index].TargetPosition = hit.point;
                }
                else if (legs[(index + legCount - 1) % legCount].TargetPosition == null
                         || legs[(index + 1) % legCount].TargetPosition == null)
                {
                    var distance = Vector3.Distance(hit.point, legs[index].target2.Position);
                    if (distance > currentStepLength)
                    {
                        StartCoroutine(StartMovement(index, hit.point));
                    }
                }

                averageNormal += hit.normal;
                averagePosition += hit.point;
            }
        }

        averagePosition /= legs.Length;
        averageNormal /= legs.Length;
        averageNormal.Normalize();
        
        var inputMovementRaw = inputActions.Spider.Movement.ReadValue<Vector2>();
        inputMovement = new Vector3(inputMovementRaw.x, 0, inputMovementRaw.y);
        inputRotation = inputActions.Spider.Rotation.ReadValue<float>();
        
        var raycastsPlaneTransform = raycastsPlane.transform;
        raycastsPlaneTransform.rotation = Quaternion.LookRotation(transform.forward, averageNormal);
        var groundPlaneVelocity = Vector3.ProjectOnPlane(rigidbody.velocity, averageNormal);
        raycastsPlaneTransform.position = transform.position + groundPlaneVelocity * raycastsOffset;
        
        var directionToFloor = Vector3.Project(averagePosition - transform.position, -averageNormal);
        var projectionOnFloor = transform.position + directionToFloor;
        var targetPosition = projectionOnFloor + averageNormal * heightAboveGround;

        var screenRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        var groundPlane = new Plane(averageNormal, transform.position);
        if (groundPlane.Raycast(screenRay, out var mouseTarget))
        {
            var vector3 = screenRay.GetPoint(mouseTarget);
            movementTarget.rotation = Quaternion.LookRotation(vector3 - transform.position, averageNormal);

        }

        if (Physics.Raycast(screenRay, out var mouseHit))
        {
            this.mouseTarget.position = mouseHit.point;
        }
        
        var groundForward = Vector3.ProjectOnPlane(mainCamera.transform.forward, averageNormal).normalized;
        var groundRight = Vector3.Cross(averageNormal, groundForward);
        var worldInput = inputMovementRaw.x * groundRight + inputMovementRaw.y * groundForward;
        movementTarget.position = targetPosition + inputMovementRaw.x * groundRight + inputMovementRaw.y * groundForward;
        movementTarget.rotation = Quaternion.AngleAxis(groundPlaneVelocity.magnitude * Lean, Vector3.Cross(averageNormal, worldInput)) * movementTarget.rotation;
    }

    private IEnumerator StartMovement(int legIndex, Vector3 position)
    {
        //Debug.Log($"Start movement {legIndex} to {position}");

        legs[legIndex].TargetPosition = position;

        Vector3 from = legs[legIndex].target2.Position;
        float t = 0.0f;
        while (t < 1)
        {
            var leg = legs[legIndex];
            t += Time.deltaTime * speed;
            leg.target2.Position = Vector3.Lerp(@from, leg.TargetPosition.Value, t) + transform.up * (Mathf.Pow(1 - Mathf.Abs(0.5f - t) * 2.0f, 2) * stepHeight);
            yield return null;
        }

        legs[legIndex].TargetPosition = null;

        //Debug.Log("Movement completed " + legIndex);
    }

    [Serializable]
    private struct LegData
    {
        public Transform RaycastOrigin;
        public LegTarget2 target2;
        [NonSerialized] public Vector3? TargetPosition;
    }
}