using UnityEngine;
using System.Collections;

public class RefillPickup : Pickup {
    public Color color;
    public float amount = 0.5f;

    public void Update()
    {
        transform.localRotation *= Quaternion.Euler(0, Time.deltaTime, 0);
    }

    protected override bool PickUp(CarTrunk trunk)
    {
        trunk.Refill(color, amount);
        return true;
    }
}
