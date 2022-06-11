using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] 
    private GameObject toSpawn;

    [SerializeField] 
    private float radius;

    [SerializeField] 
    private float count;

    [SerializeField] 
    private Vector2 scaleRange;


    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            var position = transform.position + Random.insideUnitSphere * radius;
            var scale = Random.Range(scaleRange.x, scaleRange.y);
            var rotation = Quaternion.Euler(
                Random.Range(0, 360),
                Random.Range(0, 360),
                Random.Range(0, 360));

            var o = Instantiate(toSpawn, position, rotation);
            o.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}