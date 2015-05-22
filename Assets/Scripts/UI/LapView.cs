using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LapView : MonoBehaviour {
    private RaceController _race;
    private Cockpit _myCar;


    public Text lapNumber;
    public Text lapCount;

    // Use this for initialization
	void Start () {
        _race = RaceController.Locate();
        _myCar = _race.MyCar;
	}
	
	// Update is called once per frame
	void Update () {
        lapNumber.text = Mathf.Clamp(_myCar.Laps+1, 1, _race.Laps).ToString();
        lapCount.text = _race.Laps.ToString();
	}
}
