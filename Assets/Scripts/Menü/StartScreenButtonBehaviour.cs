using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class StartScreenButtonBehaviour : MonoBehaviour {

   



    public void OnStart()
    {
        Application.LoadLevel("Lobby");
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
