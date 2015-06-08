using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LapMonitor : MonoBehaviour {

    public Slider slider;

    private Text _text;

	// Use this for initialization
	void Start () {
        _text = GetComponent<Text>();
        slider.onValueChanged.AddListener(UpdateText);

        UpdateText(slider.value);
	}

    private void UpdateText(float arg0)
    {
        _text.text = string.Format("{0:0} Laps", arg0);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
