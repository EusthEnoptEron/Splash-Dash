using UnityEngine;
using System.Collections;

public class TurboEffect : Effect {
    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponentInParent<Rigidbody>();
    }

    protected void Update()
    {
        //rigidbody.velocity += Time.deltaTime * 10 * transform.forward;
        rigidbody.velocity = transform.forward * 50;
    }

    protected override void OnEnable()
    {
    }

    protected override void OnDisable()
    {
    }
}
