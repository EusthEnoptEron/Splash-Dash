using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CarTrunk : MonoBehaviour {

    private Dictionary<Color, PaintTank> _tankMap = new Dictionary<Color, PaintTank>();

	// Use this for initialization
	void Start () {
        _tankMap = GetComponentsInChildren<PaintTank>().ToDictionary(
            tank => tank.color,
            tank => tank
        );
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    internal void Refill(Color color, float amount)
    {
        PaintTank tank;
        if(_tankMap.TryGetValue(color, out tank)) {
            tank.fillLevel = Mathf.Clamp01(tank.fillLevel + amount);
        }
    }
}
