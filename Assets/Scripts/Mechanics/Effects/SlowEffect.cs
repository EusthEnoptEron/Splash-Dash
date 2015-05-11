using UnityEngine;
using System.Collections;

public class SlowEffect : Effect {
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
        car.MaxSpeed = maxSpeed * 0.7f;
        rigidbody.velocity *= 0.5f;
    }

    protected override void OnDisable()
    {
        car.MaxSpeed = maxSpeed;
    }
}
