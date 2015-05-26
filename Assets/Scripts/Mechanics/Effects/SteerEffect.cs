using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class SteerEffect : Effect {
    private Rigidbody _rigidbody;
    private Cockpit _car;

    private float maxSpeed;
    private void Awake()
    {
        _rigidbody = GetComponentInParent<Rigidbody>();
        _car = GetComponentInParent<Cockpit>();
    }

    protected void Update()
    {

    }

    protected override void OnEnable()
    {
        _car.Inverted = true;
    }

    protected override void OnDisable()
    {
        _car.Inverted = false;

    }
}
