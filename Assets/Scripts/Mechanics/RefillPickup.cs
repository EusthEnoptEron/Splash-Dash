using UnityEngine;
using System.Collections;
using System.Linq;

public class RefillPickup : Pickup {
    public Color color;

    public float amount = 0.5f;
    public float anglePerSecond = 60f;


    public void Start()
    {
        foreach (var mat in GetComponentsInChildren<MeshRenderer>().SelectMany(r => r.materials))
        {
            if (mat.shader.name.Contains("Vertex"))
            {
                mat.color = color;
            }
        }
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
