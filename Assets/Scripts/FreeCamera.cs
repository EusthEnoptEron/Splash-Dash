using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FreeCamera : MonoBehaviour {
    private WaypointContainer _circuit;
    private float _elapsed = 0;
    public float duration = 10f;
    private float _speed = 0;
    public Vector3 offset = Vector3.zero;
    public Vector3 baseRotation = Vector3.zero;
    public int smoothIterations = 5;

    private Quaternion _baseRotation;


    private Quaternion[] _rotations;
    private Vector3[] _positions;

	// Use this for initialization
	void Start () {
        _circuit = GameObject.FindObjectOfType<WaypointContainer>();
        _baseRotation = Quaternion.Euler(baseRotation);

        _positions = _circuit.waypoints.Select(wp => wp.transform.position).ToArray().Smooth(smoothIterations, true);
        _rotations = _circuit.waypoints.Select(wp => wp.transform.rotation).ToArray().Smooth(smoothIterations, true);

        _speed = _positions.Length / Mathf.Clamp(duration, 0.01f, 1000);
        Debug.LogFormat("Smoothed to {0} points", _positions.Length);
	}
	
	// Update is called once per frame
    void Update()
    {
        _speed = _positions.Length / Mathf.Clamp(duration, 0.01f, 1000);

        int currentIndex = (int)(_elapsed) % _positions.Length;
        int nextIndex = (currentIndex + 1) % _positions.Length;
        float progress = (_elapsed) - (int)(_elapsed);

        //Debug.LogFormat("{0}->{1} ({2})", currentIndex, nextIndex, progress);
        transform.position = Vector3.Lerp(_positions[currentIndex], _positions[nextIndex], progress) + offset;
        transform.rotation = Quaternion.Slerp(_rotations[currentIndex], _rotations[nextIndex], progress) * _baseRotation;

        _elapsed += Time.deltaTime * _speed;
    }

}
