using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class RaceController : MonoBehaviour {
    public GameObject prefDigit;
    public Animator countdownGUI;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartRace()
    {
        StartCoroutine(RaceCoroutine());
    }

    private IEnumerator RaceCoroutine()
    {
        yield return StartCoroutine(DoCountdown());
    


        yield return null;
    }

    private IEnumerator DoCountdown()
    {
        countdownGUI.SetTrigger("Start");

        yield return new WaitForSeconds(2);

        SendText("3", Color.red);

        yield return new WaitForSeconds(1);

        SendText("2", Color.green);

        yield return new WaitForSeconds(1);

        SendText("1", Color.blue);

        yield return new WaitForSeconds(1);

        SendText("GO!", Color.white);
        countdownGUI.SetBool("Done", true);

        yield return new WaitForSeconds(1);


    }

    private void SendText(string text, Color color)
    {
        var digit = GameObject.Instantiate<GameObject>(prefDigit);
        digit.transform.SetParent(countdownGUI.transform, false);

        var digitText = digit.GetComponentInChildren<Text>();

        digitText.text = text;
        digitText.color = color;

        var outline = digitText.GetComponent<Outline>();
        if (outline)
        {
            outline.effectColor = color - new Color(0.3f, 0.3f, 0.3f);
        }
    }
}
