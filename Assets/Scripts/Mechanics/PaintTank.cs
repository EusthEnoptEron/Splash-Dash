using UnityEngine;
using System.Collections;

public class PaintTank : MonoBehaviour {
    public Color color;
    public float fillLevel = 1;
    public float drainSpeed = 0.1f;
    public string button = "Fire1";
    
    public bool Shooting { get { return Input.GetButton(button); } }
    public bool IsEmpty { get { return fillLevel == 0; } }
    private void Update()
    {
        if (Shooting)
        {
            fillLevel = Mathf.Clamp01(fillLevel - Time.deltaTime * drainSpeed);
        }
    }
}
