using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class EffectController : MonoBehaviour {
    private PaintBrush _paintBrush;
    private CarController _car;
    private Effect[] _effects;

    public Effect ActiveEffect { get; private set; }

	// Use this for initialization
	void Awake () {
        _effects = GetComponentsInChildren<Effect>();
        foreach (var effect in _effects)
        {
            effect.enabled = false;
        }
	}
    void Start()
    {
        _paintBrush = PaintBrush.Locate();
        _car = GetComponentInParent<CarController>();
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(_car.IsGrounded);
        if (_car.IsGrounded)
        {
            var currentColor = _paintBrush.GetColor(transform.position);

            ActiveEffect = null;

            // First, disable all effects that are inactive
            foreach (var effect in _effects)
            {
                if (!CompareColor(currentColor, effect.color))
                    effect.enabled = false;
                else
                    ActiveEffect = effect;
            }

            // Now, enable active effect
            if (ActiveEffect != null)
                ActiveEffect.enabled = true;
        }
        //else
        //{
        //    foreach (var effect in _effects)
        //    {
        //        effect.enabled = false;
        //    }
        //}
	}


    bool CompareColor(Color c1, Color c2)
    {
        return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b;
    }
}
