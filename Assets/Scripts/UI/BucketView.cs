using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class BucketView : MonoBehaviour {
    public Color observedColor;

    private PaintTank _bucket;
    private Image _fill;

	// Use this for initialization
	void Start () {
        _bucket = RaceController.Locate().MyCar
            .GetComponentsInChildren<PaintTank>().First(
            bucket => bucket.color == observedColor
        );

        _fill = transform.GetChild(0).GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        _fill.fillAmount = _bucket.fillLevel;
	}
}
