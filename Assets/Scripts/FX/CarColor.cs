using UnityEngine;
using System.Collections;
using System.Linq;
using UnityStandardAssets.Vehicles.Car;

public class CarColor : MonoBehaviour {
    public EffectController effectController;
    private Material[] _materials;
    private CarController _car;

	// Use this for initialization
	void Start () {
        _materials = GetComponentsInChildren<MeshRenderer>().SelectMany(r => r.materials).Where(mat => !mat.name.Contains("NoColor")).ToArray();
        _car = GetComponentInParent<CarController>();
	}
	
	// Update is called once per frame
	void Update () {

        var color = effectController.ActiveEffect == null 
                ? Color.white
                : effectController.ActiveEffect.color;

        if (color.a == 0)
        {
            foreach (var mat in _materials) mat.color = Color.Lerp(mat.color, Color.white, Time.deltaTime * 10);
        }
        else
        {
            foreach (var mat in _materials) mat.color = Color.Lerp(mat.color, color, Time.deltaTime * 10); ;
        }
	}
}
