using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pulsator : MonoBehaviour
{

    public float Interval;
    public Vector3 Impulse;

    IEnumerator Start()
    {
        var rigidbody = GetComponent<Rigidbody>();
        while (true)
        {
            yield return new WaitForSeconds(Interval);
            rigidbody.AddForce(transform.TransformDirection(Impulse), ForceMode.VelocityChange);
        }
    }

}