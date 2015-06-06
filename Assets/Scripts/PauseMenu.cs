using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {


    public GameObject PauseCanvas;


	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Escape)){

            
            if (PauseCanvas.activeSelf)
	        {
               
	            PauseCanvas.SetActive(false);
                
   	        }
            else
            {
                PauseCanvas.SetActive(true);
               //TODO: Disable Carcontroll
            }

        }
	}


    public void OnPauseReturn()
    {
        Application.LoadLevel("Menü");

    }

    public void OnPauseResume()
    {
        PauseCanvas.SetActive(false);
    }
}
