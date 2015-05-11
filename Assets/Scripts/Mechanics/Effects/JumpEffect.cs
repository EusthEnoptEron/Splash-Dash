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
        rigidbody.AddForce(Vector3.up * 5000);
    }

    protected override void OnDisable()
    {
    }
}
