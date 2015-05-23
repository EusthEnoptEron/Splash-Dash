using UnityEngine;
using System.Collections;

public class PrefabSpawner : MonoBehaviour {
    public GameObject prefab;

    public bool repeat = true;
    public float interval = 30;
    private float _lastLifeSign = 0;


    private GameObject currentInstance;


	// Use this for initialization
	void Awake () {
        enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (!currentInstance && repeat && Time.time - _lastLifeSign > interval)
        {
            Spawn();
        }
        else if (currentInstance)
        {
            _lastLifeSign = Time.time;
        }
	}

    private void Spawn()
    {
        currentInstance = Network.Instantiate(prefab, transform.position, transform.rotation, 0) as GameObject;
        currentInstance.transform.SetParent(transform, true);
    }


    void OnServerInitialized()
    {
        // We're the server, baby!
        enabled = true;

        Spawn();
    }
}
