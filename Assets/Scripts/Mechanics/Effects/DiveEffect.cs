using UnityEngine;
using System.Collections;

public class DiveEffect : Effect {
    private Cockpit _car;

    private int _originalLayer;
    private int _ghostLayer;
	// Use this for initialization
	void Awake () {
        _car = GetComponentInParent<Cockpit>();
        _originalLayer = _car.gameObject.layer;
        _ghostLayer = LayerMask.NameToLayer("Ghost");

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected override void OnEnable()
    {
        foreach (var child in _car.gameObject.GetComponentsInChildren<Transform>())
        {
            if(child.name != "RoadTester")
                child.gameObject.layer = _ghostLayer;
        }
                
    }

    protected override void OnDisable()
    {
        foreach (var child in _car.gameObject.GetComponentsInChildren<Transform>())
            child.gameObject.layer = _originalLayer;
    }
}
