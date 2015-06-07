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
            var raceController = RaceController.Locate();

            if (raceController.State < RaceState.Running)
                return;

            if (PauseCanvas.activeSelf)
	        {
	            PauseCanvas.SetActive(false);
                raceController.SetPaused(false);
   	        }
            else
            {
                PauseCanvas.SetActive(true);
                raceController.SetPaused(true);
               //TODO: Disable Carcontroll
            }

        }
	}


    public void OnPauseReturn()
    {
        Network.Disconnect();
        Application.LoadLevel("Menü");
    }

    public void OnPauseResume()
    {
        PauseCanvas.SetActive(false);
    }
}
