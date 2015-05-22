using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PositionView : MonoBehaviour {


    private RaceController _race;
    private Cockpit _myCar;

    public Text textEl;

	// Use this for initialization
	void Start () {
        _race = RaceController.Locate();
        _myCar = _race.MyCar;
	}
	
	// Update is called once per frame
	void Update () {
        textEl.text = _race.GetRank(_myCar).ToSuffixedString();
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
