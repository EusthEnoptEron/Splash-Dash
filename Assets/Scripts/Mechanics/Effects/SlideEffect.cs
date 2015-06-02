using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class SlideEffect : Effect
{
    private float[] defaultSlips = new float[4];
    private WheelCollider[] wheels = new WheelCollider[4];
    private const float SlipModifier = 100;

    private void Awake()
    {
        int i = 0;
        foreach (WheelCollider c in GetComponentInParent<CarController>().Wheels)
        {
            defaultSlips[i] = c.sidewaysFriction.stiffness;
            wheels[i] = c;
            i++;
        }

    }

    protected void Update()
    {
        //rigidbody.velocity += Time.deltaTime * 10 * transform.forward;

    }

    protected override void OnEnable()
    {
        foreach (WheelCollider c in this.wheels)
        {
            WheelFrictionCurve frictionCurve = c.sidewaysFriction;
            frictionCurve.stiffness /= SlipModifier;
            c.sidewaysFriction = frictionCurve;
        }
    }

    protected override void OnDisable()
    {
        int i = 0;
        foreach (WheelCollider c in this.wheels)
        {
            WheelFrictionCurve frictionCurve = c.sidewaysFriction;
            frictionCurve.stiffness = this.defaultSlips[i];
            c.sidewaysFriction = frictionCurve;
            i++;
        }
    }
}
