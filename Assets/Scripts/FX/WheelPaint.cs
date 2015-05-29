using UnityEngine;
using System.Collections;

public class WheelPaint : MonoBehaviour {

    private GameObject pref_PaintContact;

    private WheelCollider wheelCollider;
    private ParticleSystem particleSystem;
    private PaintBrush paintBrush;

	// Use this for initialization
	void Start () {

        pref_PaintContact = Resources.Load<GameObject>("Prefabs/pref_PaintContact");
        particleSystem = GameObject.Instantiate<GameObject>(pref_PaintContact).GetComponent<ParticleSystem>();
        particleSystem.enableEmission = false;

        wheelCollider = GetComponent<WheelCollider>();
        paintBrush = PaintBrush.Locate();
	}
	
	// Update is called once per frame
	void Update () {

        WheelHit groundHit;
        if (wheelCollider.GetGroundHit(out groundHit))
        {
            //Vector3 pos; Quaternion rot;
            //wheelCollider.GetWorldPose(out pos, out rot);

            particleSystem.transform.position = groundHit.point;
            particleSystem.transform.rotation = Quaternion.LookRotation(transform.parent.forward, transform.parent.up);
            var color = paintBrush.GetColor(transform.position);
            if (color.a == 1)
            {
                //color.r += 0.5f;
                //color.g += 0.5f;
                //color.b += 0.5f;

                particleSystem.startColor = color;
                particleSystem.enableEmission = true;
            }
            else
            {
                particleSystem.enableEmission = false;
            }
        }
        else
        {
            particleSystem.enableEmission = false;
        }
	}
}
