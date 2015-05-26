using UnityEngine;
using System.Collections;

public class FreeCamera : MonoBehaviour {

    private WaypointContainer _circuit;
    private float _elapsed = 0;
    public float speed = 0.1f;
    public Vector3 offset = Vector3.zero;
    public Vector3 baseRotation = Vector3.zero;

    private Quaternion _baseRotation;

	// Use this for initialization
	void Start () {
        _circuit = GameObject.FindObjectOfType<WaypointContainer>();
        _baseRotation = Quaternion.Euler(baseRotation);
	}
	
	// Update is called once per frame
	void Update () {
        int currentIndex = (int)(_elapsed * speed) % _circuit.waypoints.Count;
        int nextIndex = (currentIndex + 1) % _circuit.waypoints.Count;
        float progress = (_elapsed * speed) - (int)(_elapsed * speed);

        transform.position = Vector3.Lerp(_circuit.waypoints[currentIndex].transform.position, _circuit.waypoints[nextIndex].transform.position, progress) + offset;
        transform.rotation = Quaternion.Slerp(_circuit.waypoints[currentIndex].transform.rotation, _circuit.waypoints[nextIndex].transform.rotation, progress) * _baseRotation;

        _elapsed += Time.deltaTime;
	}
}
