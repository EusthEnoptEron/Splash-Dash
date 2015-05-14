using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class FillSphere : MonoBehaviour {

    public float radius = 1;
    public float fillState = 1;

    public int widthSegments = 8;
    public int heightSegments = 6;
    private MeshFilter meshFilter;


	// Use this for initialization
	void Start () {
	    meshFilter = GetComponent<MeshFilter>();

	}

    void Update()
    {
        BuildMesh();
    }

    void BuildMesh()
    {
        fillState = Mathf.Clamp01(fillState);
        widthSegments = (int)Mathf.Max( 3, widthSegments);
	    heightSegments = (int)Mathf.Max( 2, heightSegments);

	    float phiStart = 0;
	    float phiLength = Mathf.PI * 2;

	    float thetaStart =  0;
	    float thetaLength = Mathf.PI;

	    int x, y;
        var thisVertices = new List<Vector3>();
        var vertices = new int[heightSegments+1, widthSegments+1];
        var faces = new List<int>();
        int lastSegment = heightSegments+1;

        var uvs = new List<Vector2[]>();

	    for ( y = 0; y <= heightSegments; y ++ ) {
            float v = (float)y / heightSegments;
            if (v > fillState)
            {
                thisVertices.Add(new Vector3(0, thisVertices.Last().y, 0));
                break;
            }
            else
                lastSegment = y;

		    var uvsRow = new Vector2[widthSegments+1];

		    for ( x = 0; x <= widthSegments; x ++ ) {

			    float u = (float)x / widthSegments;

			    var vertex = new Vector3();
			    vertex.x = - radius * Mathf.Cos( phiStart + u * phiLength ) * Mathf.Sin( thetaStart + v * thetaLength );
			    vertex.y =  radius * -Mathf.Cos( thetaStart + v * thetaLength );
			    vertex.z = radius * Mathf.Sin( phiStart + u * phiLength ) * Mathf.Sin( thetaStart + v * thetaLength );

			    thisVertices.Add( vertex );

                vertices[y, x] = thisVertices.Count - 1;
                uvsRow[x] = new Vector2( u, 1 - v );

		    }
		    uvs.Add( uvsRow );
	    }

	    for ( y = 0; y < heightSegments; y ++ ) {

            if (y >= lastSegment)
            {
                if (y != 0)
                {
                    // Close
                    for (x = 0; x < widthSegments; x++)
                    {

                        var v2 = vertices[y - 1, x];
                        var v1 = v2 - 1;
                        var v3 = v2 + 1;

                        var center = thisVertices.Count() - 1;

                        if (v3 == 0)
                        {
                            faces.AddRange(new int[] { center, v1, v2 });
                        }
                        else
                        {
                            faces.AddRange(new int[] { center, v2, v3 });
                        }
                        //faces.AddRange(new int[] { v3, v1, v4 });
                    }
                }

            }
            else
            {

                for (x = 0; x < widthSegments; x++)
                {

                    var v1 = vertices[y, x + 1];
                    var v2 = vertices[y, x];
                    var v3 = vertices[y + 1, x];
                    var v4 = vertices[y + 1, x + 1];

                    var n1 = thisVertices[v1].normalized;
                    var n2 = thisVertices[v2].normalized;
                    var n3 = thisVertices[v3].normalized;
                    var n4 = thisVertices[v4].normalized;

                    var uv1 = uvs[y][x + 1];
                    var uv2 = uvs[y][x];
                    var uv3 = uvs[y + 1][x];
                    var uv4 = uvs[y + 1][x + 1];

                    if (Mathf.Abs(thisVertices[v1].y) == radius)
                    {

                        uv1.x = (uv1.x + uv2.x) / 2;
                        faces.AddRange(new int[] { v3, v1, v4 });

                    }
                    else if (Mathf.Abs(thisVertices[v3].y) == radius)
                    {

                        uv3.x = (uv3.x + uv4.x) / 2;
                        faces.AddRange(new int[] { v2, v1, v3 });

                    }
                    else
                    {

                        faces.AddRange(new int[] { v2, v1, v4 });

                        faces.AddRange(new int[] { v3, v2, v4 });

                    }

                }
            }

	    }

        var mesh = new Mesh();
        mesh.vertices = thisVertices.ToArray();
        mesh.triangles = faces.ToArray();

        meshFilter.mesh = mesh;
    }

}
