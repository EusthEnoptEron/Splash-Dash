using UnityEngine;
using System.Collections;

public class BirdsEye : MonoBehaviour {

    public Transform target;
    public float height = 100;
    public Vector3 normal = Vector3.up;
    public Transform gizmo;

	// Update is called once per frame
	void Update () {
        if (target)
        {
            var pos = Vector3.ProjectOnPlane(target.position, normal);
            transform.position = pos + height * normal;
            transform.rotation = Quaternion.LookRotation(-normal, Camera.main.transform.forward);

            if (gizmo)
            {
                gizmo.transform.position = pos + normal * height / 2;
                //gizmo.transform.rotation = Quaternion.LookRotation(normal);
            }
        }
        
	}
}
