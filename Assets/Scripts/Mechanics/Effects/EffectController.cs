using UnityEngine;
using System.Collections;

public class EffectController : MonoBehaviour {
    public PaintBrush paintBrush;
    private Effect[] _effects;

	// Use this for initialization
	void Awake () {
        _effects = GetComponentsInChildren<Effect>();
        foreach (var effect in _effects)
        {
            effect.enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
        var currentColor = paintBrush.GetColor(transform.position);

        foreach (var effect in _effects)
        {
            effect.enabled = CompareColor(currentColor, effect.color);
        }
	}

    bool CompareColor(Color c1, Color c2)
    {
        return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b;
    }
}
