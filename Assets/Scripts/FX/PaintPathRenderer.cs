using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class PaintPathRenderer : MonoBehaviour {

    public int vertexCount = 50;
    public float uvSpeed = 1;
    
    public SpurtEmitter emitter;
    private Renderer renderer;
    private MeshFilter meshFilter;

	// Use this for initialization
	void Awake () {
        renderer = GetComponent<Renderer>();
        meshFilter = GetComponent<MeshFilter>();

  
	}

    void Start()
    {
        if (emitter.IsRemoteControlled)
        {
            GameObject.Destroy(this);
            GameObject.Destroy(renderer);
        }
        else
        {
            //renderer.SetVertexCount(vertexCount);
            //renderer.useWorldSpace = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
        var direction = emitter.transform.forward;
        var velocity  = emitter.startVelocity;
        var vertices = new List<Vector3>();

        for (int i = 0; i < vertexCount; i++)
        {
            float t = (i + ((Time.time * uvSpeed) % 2)) * 0.01f;
            //renderer.SetPosition(i, emitter.transform.position + (direction * velocity + emitter.Inertia) * t + 0.5f * Physics.gravity * t * t);
            vertices.Add(transform.InverseTransformPoint( emitter.transform.position + (direction * velocity + emitter.Inertia) * t + 0.5f * Physics.gravity * t * t ));
        }

        renderer.material.mainTextureOffset += new Vector2(uvSpeed * Time.deltaTime, 0);

        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        //mesh.SetIndices(vertices.Select((v, i) => i).ToArray(), MeshTopology.LineStrip, 0);
        mesh.SetIndices(vertices.Take(vertices.Count-1).Select((v, i) => i).Where(i => i % 2 == 0).SelectMany(i => new int[]{i, i+1}).ToArray(), MeshTopology.Lines, 0);

        meshFilter.mesh = mesh;
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
