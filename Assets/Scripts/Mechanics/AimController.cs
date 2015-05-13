using UnityEngine;
using System.Collections;

public class AimController : MonoBehaviour {
    public float maxSteer = 50;
    public float maxTilt = 50;
    SpurtEmitter emitter;
	// Use this for initialization
	void Start () {
        emitter = GetComponent<SpurtEmitter>();

	}
	
	// Update is called once per frame
	void Update () {

        if (!emitter.IsRemoteControlled)
        {
            float steer = Mathf.Clamp01(Input.mousePosition.x / Screen.width) - 0.5f;
            float tilt = Mathf.Clamp01(Input.mousePosition.y / Screen.height) - 0.5f;
            emitter.startVelocity = Mathf.Clamp(emitter.startVelocity + Input.GetAxis("Mouse ScrollWheel") * 5, 5, 50);

            emitter.transform.localRotation = Quaternion.Euler(0, steer * maxSteer, 0) * Quaternion.Euler(-tilt * maxTilt, 0, 0);
        }
	}
}
