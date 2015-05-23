using UnityEngine;
using System.Collections;

public class DangerView : MonoBehaviour {

    private Cockpit _car;
    private Animator _animator;

    private float _onRoadTime = 0;
    public float timeToRespawn = 3;

	// Use this for initialization
	void Start () {
        _car = RaceController.Locate().MyCar;
        _animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        _animator.SetBool("onRoad", _car.isOnRoad);

        if (_car.isOnRoad) _onRoadTime = Time.time;
        if (Time.time - _onRoadTime > timeToRespawn)
        {
            _car.Respawn();
        }
	}
}
