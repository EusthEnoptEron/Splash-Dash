using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpurtEmitter), typeof(LineRenderer))]
public class PaintPathRenderer : MonoBehaviour {

    public int vertexCount = 50;

    private SpurtEmitter emitter;
    private LineRenderer renderer;
	// Use this for initialization
	void Awake () {
        emitter = GetComponent<SpurtEmitter>();
        renderer = GetComponent<LineRenderer>();

        renderer.SetVertexCount(vertexCount);
        renderer.useWorldSpace = true;
	}
	
	// Update is called once per frame
	void Update () {
        var direction = emitter.transform.forward;
        var velocity  = emitter.startVelocity;

        for (int i = 0; i < vertexCount; i++)
        {
            float t = i * 0.1f;
            renderer.SetPosition(i, emitter.transform.position + (direction * velocity + emitter.Inertia) * t + 0.5f * Physics.gravity * t * t);
        }

        renderer.material.mainTextureOffset += new Vector2(-10 * Time.deltaTime, 0);
	}

    void OnEnable()
    {
        renderer.enabled = true;
    }

    void OnDisable()
    {
        renderer.enabled = false;
    }
}
