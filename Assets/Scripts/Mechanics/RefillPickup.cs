using UnityEngine;
using System.Collections;

public class RefillPickup : Pickup {
    public Color color;
    public Material material;

    public float amount = 0.5f;
    public float anglePerSecond = 60f;


    public void Start()
    {
        if (material)
            material.color = color;
    }

    public void Update()
    {
        transform.localRotation *= Quaternion.Euler(0, Time.deltaTime * anglePerSecond, 0);
    }

    protected override bool PickUp(CarTrunk trunk)
    {
        trunk.Refill(color, amount);
        return true;
    }
}
