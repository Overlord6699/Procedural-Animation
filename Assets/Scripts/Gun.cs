using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ParticleSystem))]
public class Gun : MonoBehaviour
{
    [SerializeField]
    private float shotsPerSecond = 10;

    [SerializeField]
    private Rigidbody recoilTarget;

    [SerializeField][Header("Отдача")]
    private float recoilImpulse;
    
    private ParticleSystem fireParticles;
    private InputAction fire;
        
    private void Awake()
    {
        fire = new InputAction("Fire", InputActionType.Button, "Mouse/leftButton");
        fire.Enable();
        fireParticles = GetComponent<ParticleSystem>();

        fire.started += FireStarted;
        fire.canceled += FireCompleted;
    }

    private void FireStarted(InputAction.CallbackContext obj)
    {
        StartCoroutine(Fire());
    }

    private void FireCompleted(InputAction.CallbackContext obj)
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        fire.Dispose();
    }

    private IEnumerator Fire()
    {
        var interval = 1 / shotsPerSecond;
        var t = interval;

        while (true)
        {
            t += Time.deltaTime;

            if (t > interval)
            {
                t -= interval;
                fireParticles.Emit(1);
                //немного физики
                recoilTarget.AddForceAtPosition(-transform.forward * recoilImpulse, transform.position, ForceMode.Impulse);
            }

            yield return null;
        }
    }
}