using UnityEngine;

public class LegTarget : MonoBehaviour
{
    [SerializeField] private float stepSpeed = 5.0f;

    [SerializeField] private AnimationCurve stepCurve;

    private Vector3 position;
    private Movement? movement;
    private Transform transform;

    public Vector3 Position => position;
    public bool IsMoving => movement != null;

    private void Awake()
    {
        transform = base.transform;
        position = transform.position;
    }

    private void Update()
    {
        if (movement != null)
        {
            var m = movement.Value;
            m.Progress = Mathf.Clamp01(m.Progress + Time.deltaTime * stepSpeed);
            position = m.Evaluate(Vector3.up, stepCurve);
            movement = m.Progress < 1 ? m : null;
        }

        transform.position = position;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        if (movement == null)
            movement = new Movement
            {
                Progress = 0,
                FromPosition = position,
                ToPosition = targetPosition
            };
        else
            movement = new Movement
            {
                Progress = movement.Value.Progress,
                FromPosition = movement.Value.FromPosition,
                ToPosition = targetPosition
            };
    }

    private struct Movement
    {
        public float Progress;
        public Vector3 FromPosition;
        public Vector3 ToPosition;

        public Vector3 Evaluate(in Vector3 up, AnimationCurve stepCurve)
        {
            return Vector3.Lerp(@FromPosition, ToPosition, Progress) + up * stepCurve.Evaluate(Progress);
        }
    }
}