using UnityEngine;
using System.Collections;

public class TurboEffect : Effect {
    private Rigidbody rigidbody;
    private CarController car;

    private float maxSpeed;
    private void Awake()
    {
        rigidbody = GetComponentInParent<Rigidbody>();
        car = GetComponentInParent<CarController>();

        maxSpeed = car.MaxSpeed;
    }

    protected void Update()
    {
        //rigidbody.velocity += Time.deltaTime * 10 * transform.forward;
 
    }

    protected override void OnEnable()
    {
        car.MaxSpeed = maxSpeed * 1.5f;
        rigidbody.velocity *= 1.5f;
    }

    protected override void OnDisable()
    {
        car.MaxSpeed = maxSpeed;
    }
}
