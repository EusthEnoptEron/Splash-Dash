using UnityEngine;
using System.Collections;

public class DontDestroyScritp : MonoBehaviour {


    private static bool audioPlaying = false;
    void Awake()
    {

        if (!audioPlaying)
        {
            print("yip");
            audioPlaying = true;
            DontDestroyOnLoad(transform.gameObject);
            
        }
        else
        {
            Destroy(transform.gameObject);
            
        }

        

        
    }



}
