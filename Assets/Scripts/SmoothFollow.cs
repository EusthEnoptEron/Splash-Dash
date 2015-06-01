using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SmoothFollow : MonoBehaviour {
	public Transform target;            // The target we are following
	public float distance = 10.0f;      // The distance in x-z plane to the target
	public float height = 2.0f;         // the height of the camera above the target
	public float heightDamping = 2.0f;  // How much we damp in height 
	public float rotationDamping= 1.0f; // How much we damp in rotation 

    public float minDistance = 5;
    public float maxDistance = 10;
    public float minSpeed = 5;
    public float maxSpeed = 20;

	void LateUpdate () {
		// Early out if we don't have a target
		if (!target) return;

        var rigidbody = target.GetComponent<Rigidbody>();
        if(rigidbody)
            distance = Mathf.Lerp(distance, Mathf.Lerp(minDistance, maxDistance, Mathf.Clamp01((rigidbody.velocity.magnitude - minSpeed) / (maxSpeed - minSpeed))), Time.deltaTime);

		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;

        // Calculate the current rotation angles
        float wantedHeight = target.position.y + height;

        RaycastHit hit;
        int i = 0;
        float wantedRotationAngle = RotationAngles.FirstOrDefault(
            angle =>
            {
                var pos = PositionFromRotationAngle(angle, wantedHeight);
                var direction = (target.position - pos).normalized;
                pos -= direction * 2;
                //Debug.DrawRay(pos - direction * 5, direction, Color.green);

                if (Physics.Raycast(pos, direction, out hit))
                {
                    //Debug.LogFormat("{0} ({1})", hit.collider.name, i++);
                    return hit.collider.transform.IsChildOf(target);

                }
                return false;
            }
        );
        if (wantedRotationAngle == default(float))
        {
            wantedRotationAngle = RotationAngles.First();
        }
        //Debug.Log(wantedRotationAngle);

		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, 
		                                        wantedRotationAngle, 
		                                        rotationDamping * Time.deltaTime);
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, 
		                            wantedHeight, 
		                            heightDamping * Time.deltaTime);
		
		// Convert the angle into a rotation around the y-axis
		Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * distance;
		
		// Set the height of the camera
		transform.position = new Vector3(transform.position.x, 
		                                 currentHeight, 
		                                 transform.position.z);
		// Always look at the target
		transform.LookAt(target);
	}

    IEnumerable<float> RotationAngles
    {
        get
        {
            float targetAngle = target.eulerAngles.y;
            yield return targetAngle;

            for (int i = 5; i < 180; i += 50)
            {
                yield return targetAngle + i;
                yield return targetAngle - i;
            }

            //for (int i = 5; i < 180; i += 10)
            //{
            //    yield return targetAngle - i;
            //    //yield return targetAngle - i;
            //}
            
        }
    }

    private Vector3 PositionFromRotationAngle(float rotationAngle, float height)
    {
        // Convert the angle into a rotation around the y-axis
        Quaternion currentRotation = Quaternion.Euler(0, rotationAngle, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        var position = target.position;
        position -= currentRotation * Vector3.forward * distance;

        // Set the height of the camera
        position = new Vector3(position.x,
                            height,
                            position.z);
        return position;
    }
}
