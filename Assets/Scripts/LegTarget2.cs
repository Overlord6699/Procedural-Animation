using UnityEngine;

public class LegTarget2 : MonoBehaviour
{
    private Vector3 position;
    private Transform transform;

    public Vector3 Position
    {
        get => position;
        set => position = value;
    }

    private void Awake()
    {
        transform = base.transform;
    }

    private void Update()
    {
        transform.position = position;
    }
}