using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class SimpleControls : MonoBehaviour {
    private new Rigidbody rigidbody;
    public SpurtEmitter emitter;
    public PaintBrush paintBrush;
    public float maxSteer = 50;
    public float maxTilt = 50;
    private Material[] materials;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
        materials = transform.GetChild(0).GetComponentsInChildren<MeshRenderer>().Select(r => r.material).ToArray();
	}

    public void Update()
    {
        var color = paintBrush.GetColor(transform.position);
        if (color.a == 0)
        {
            foreach (var mat in materials) mat.color = Color.Lerp(mat.color, Color.white, Time.deltaTime * 10);
        }
        else
        {
            foreach (var mat in materials) mat.color = Color.Lerp(mat.color, color, Time.deltaTime * 10); ;
        }

        emitter.shooting = Input.GetKey(KeyCode.Space);

        float steer = Mathf.Clamp01(Input.mousePosition.x / Screen.width) - 0.5f;
        float tilt = Mathf.Clamp01(Input.mousePosition.y / Screen.height) - 0.5f;
        emitter.startVelocity = Mathf.Clamp(emitter.startVelocity + Input.GetAxis("Mouse ScrollWheel") * 5, 5, 50 );

        emitter.transform.localRotation = Quaternion.Euler(0, steer * maxSteer, 0) * Quaternion.Euler(-tilt * maxTilt, 0, 0);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        rigidbody.MoveRotation(transform.rotation * Quaternion.Euler(0, h * 50 * Time.deltaTime, 0));
        rigidbody.AddForce(v * transform.forward * Time.deltaTime * rigidbody.mass * 1000);

        if (rigidbody.velocity.magnitude / 3.6 > 80)
            rigidbody.velocity = rigidbody.velocity.normalized * 80 * 3.6f;

	}
}
