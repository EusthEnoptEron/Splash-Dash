using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PositionView : MonoBehaviour {


    private RaceController _race;
    private Cockpit _myCar;

    public Text textEl;
    private int displayedRank = 0;
    private int toDisplayRank = 0;


	// Use this for initialization
	void Start () {
        _race = RaceController.Locate();
        _myCar = _race.MyCar;

        UpdateRank();
	}
	
	// Update is called once per frame
	void Update () {
        int currentRank = _race.GetRank(_myCar);
        if (toDisplayRank != currentRank)
        {
            toDisplayRank = currentRank;
            if (displayedRank <= 0) UpdateRank();
            else
            {
                CancelInvoke("UpdateRank");
                Invoke("UpdateRank", 0.2f);
            }
        }
	}

    void UpdateRank()
    {
        displayedRank =  toDisplayRank;
        textEl.text = toDisplayRank.ToSuffixedString();
    }

}


public static class IntegerExtensions
{
    public static string ToSuffixedString(this int num)
    {
        if (num.ToString().EndsWith("11")) return num.ToString() + "th";
        if (num.ToString().EndsWith("12")) return num.ToString() + "th";
        if (num.ToString().EndsWith("13")) return num.ToString() + "th";
        if (num.ToString().EndsWith("1")) return num.ToString() + "st";
        if (num.ToString().EndsWith("2")) return num.ToString() + "nd";
        if (num.ToString().EndsWith("3")) return num.ToString() + "rd";
        return num.ToString() + "th";
    }
}
