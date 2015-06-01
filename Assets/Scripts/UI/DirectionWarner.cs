using UnityEngine;
using System.Collections;

public class DirectionWarner : MonoBehaviour {

    public Animator animator;

    private Cockpit _car;
    private WaypointContainer _circuit;

	// Use this for initialization
	void Start () {
        _car = RaceController.Locate().MyCar;
        _circuit = GameObject.FindGameObjectWithTag("Circuit").GetComponent<WaypointContainer>(); 
	}
	
	// Update is called once per frame
	void Update () {
        animator.SetBool("isCorrect", Vector3.Dot(_car.transform.forward, _circuit.GetDirection(_car.transform.position)) >= -0.6);
	}
}
