using UnityEngine;
using System.Collections;

public class CameraBeahviour : MonoBehaviour {

    public GameObject Car;
    GameObject SelectedCar;
	
    // Use this for initialization
	void Start () {
        transform.position = new Vector3(Car.transform.position.x, transform.position.y, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
