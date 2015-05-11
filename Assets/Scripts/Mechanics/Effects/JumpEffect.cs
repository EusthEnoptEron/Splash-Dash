using UnityEngine;
using System.Collections;

public class JumpEffect : Effect {
    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponentInParent<Rigidbody>();
    }


    protected override void OnEnable()
    {
        rigidbody.AddForce(Vector3.up * 500 * rigidbody.mass);
    }

    protected override void OnDisable()
    {
    }
}
