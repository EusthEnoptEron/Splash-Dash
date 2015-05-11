using UnityEngine;
using System.Collections;

public class SlowEffect : Effect {
    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponentInParent<Rigidbody>();
    }

    protected void Update()
    {
        rigidbody.velocity = transform.forward * 10;
    }

    protected override void OnEnable()
    {
    }

    protected override void OnDisable()
    {
    }
}
