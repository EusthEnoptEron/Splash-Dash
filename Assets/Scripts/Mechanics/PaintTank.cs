using UnityEngine;
using System.Collections;

public class PaintTank : MonoBehaviour {
    public Color color;
    public float fillLevel = 1;
    public float drainSpeed = 0.1f;
    public string button = "Fire1";
    public float ReleaseTime { get; set; }
    //public float delay = 0.1f;


    //private float lastPress = float.NegativeInfinity;

    //private float _shooting = 0;

    //public bool Shooting { get { return _shooting >= 0.5f; } }
    //public bool Pressed { get { return Input.GetButton(button); } }
    public bool IsEmpty { get { return fillLevel == 0; } }

    public bool IsShootable { get; private set; }

    private void Update()
    {
        if (Input.GetButtonDown(button))
        {
            IsShootable = true;
        }
        if (Input.GetButtonUp(button))
        {
            IsShootable = false;
            ReleaseTime = Time.time;
        }

    }

    //private void Update()
    //{
    //    if (Shooting)
    //    {
    //        fillLevel = Mathf.Clamp01(fillLevel - Time.deltaTime * drainSpeed);
    //    }

    //    _shooting = Pressed ? Mathf.MoveTowards(_shooting, 1, Time.deltaTime / delay) : Mathf.MoveTowards(_shooting, 0, Time.deltaTime / delay  );
    //}

    internal void Consume(float dt)
    {
        fillLevel = Mathf.Clamp01(fillLevel - dt * drainSpeed);
    }
}
