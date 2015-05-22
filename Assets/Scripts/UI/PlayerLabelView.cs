using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerLabelView : MonoBehaviour {
    public Cockpit cockpit;

	
	// Update is called once per frame
    void Start()
    {
        UpdateName();
    }


    public void UpdateName()
    {
        GetComponentInChildren<Text>().text = cockpit.name;

    }

    void Update()
    {
        if (!cockpit) DestroyImmediate(gameObject);
        else
        {
            transform.rotation = Quaternion.LookRotation((transform.position - Camera.main.transform.position).normalized);
            transform.position = cockpit.transform.position + Vector3.up;
        }
    }
}
