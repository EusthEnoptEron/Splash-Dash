using UnityEngine;
using System.Collections;

public class TitelWobbel : MonoBehaviour {


    Vector3[] verts;
    

	// Use this for initialization
	void Start () {
        verts = GetComponent<MeshFilter>().mesh.vertices;
       // StartCoroutine("CalcRandom");

      
        
	}
	
	// Update is called once per frame
	void Update () {

        var loaclverts = new Vector3[verts.Length];

        //  System.Array.Copy(verts, loaclverts, loaclverts.Length);

        for (int i = 0; i < verts.Length; i++)
        {
            float t = (Mathf.Sin((Time.time + (verts[i].x + verts[i].z + verts[i].y)*0.5f) * 0.5f * Mathf.PI * 2) + 1.0f) / 2.0f;

            loaclverts[i] = verts[i] + (verts[i].normalized * Mathf.Lerp(-0.1f, 0.3f, t));
        }

        GetComponent<MeshFilter>().mesh.vertices = loaclverts;
       
	    
	}

    //IEnumerator CalcRandom()
    //{
    //    while (true)
    //    {
    //        randomNumber = Random.Range(0, 0.2f);
    //        print(randomNumber);

    //        yield return new WaitForSeconds(3);
    //    }

    //}
}
