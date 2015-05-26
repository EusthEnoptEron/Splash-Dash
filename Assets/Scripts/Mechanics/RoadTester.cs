using UnityEngine;
using System.Collections;

public class RoadTester : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider collider) {
        Debug.Log(collider.name);
	}
}
