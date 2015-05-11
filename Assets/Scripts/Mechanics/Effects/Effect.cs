using UnityEngine;
using System.Collections;

public abstract class Effect : MonoBehaviour {
    public Color color;

    protected abstract void OnEnable();
    protected abstract void OnDisable();
}
