using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FreeCamera : MonoBehaviour {
    private delegate T Multiplier<T>(T point, float factor);
    private delegate T Summer<T>(T point1, T point2);

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

        _positions = ChaikinSmooth(_circuit.waypoints.Select(wp => wp.transform.position).ToArray(),
                            (p, k) => p * k, (p1, p2) => p1 + p2,
                            smoothIterations, true);
        _rotations = ChaikinSmooth(_circuit.waypoints.Select(wp => wp.transform.rotation).ToArray(),
                            (q, k) => Quaternion.Slerp(Quaternion.identity, q, k), (q1, q2) => q1 * q2,
                            smoothIterations, true);

        _speed = _positions.Length / Mathf.Clamp(duration, 0.01f, 1000);
        Debug.LogFormat("Smoothed to {0} points", _positions.Length);
	}
	
	// Update is called once per frame
	void Update () {
        _speed = _positions.Length / Mathf.Clamp(duration, 0.01f, 1000);

        int currentIndex = (int)(_elapsed) % _positions.Length;
        int nextIndex = (currentIndex + 1) % _positions.Length;
        float progress = (_elapsed) - (int)(_elapsed);

        //Debug.LogFormat("{0}->{1} ({2})", currentIndex, nextIndex, progress);
        transform.position = Vector3.Lerp(_positions[currentIndex], _positions[nextIndex], progress) + offset;
        transform.rotation = Quaternion.Slerp(_rotations[currentIndex], _rotations[nextIndex], progress) * _baseRotation;

        _elapsed += Time.deltaTime * _speed;
	}

    

    private T[] ChaikinSmooth<T>(T[] points, Multiplier<T> predicate, Summer<T> summer, int iterations, bool loop)
    {
        List<T> currentPoints = new List<T>(points);
        if (loop)
        {
            currentPoints.Insert(0, points.Last());
            currentPoints.Add(points.First());
        }

        for (int i = 0; i < iterations; i++)
        {
            T[] nextPoints = new T[currentPoints.Count * 2 - 2];
            for (int j = 0; j < currentPoints.Count-1; j++)
            {
                nextPoints[j * 2] = summer(predicate(currentPoints[j], 0.75f),  predicate(currentPoints[j + 1], 0.25f));
                nextPoints[j * 2 + 1] = summer(predicate(currentPoints[j], 0.25f), predicate(currentPoints[j + 1], 0.75f));
            }

            currentPoints = nextPoints.ToList();
        }

        if (loop)
        {
            currentPoints.RemoveAt(0);
            currentPoints.RemoveAt(currentPoints.Count - 1);
        }

        return currentPoints.ToArray();
    }

}
