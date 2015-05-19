using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public enum Car
{
    One = 1,
    Two = 2,
    Three = 3
}

public class SplashPlayer : NetworkBehaviour {
    public string Name;
    public NetworkPlayer Owner;
    public Car CarPrefab;
    public CarController Car;

    protected GameObject Prefab
    {
        get
        {
            return Resources.Load<GameObject>("Prefabs/Cars/pref_Car" +  (int)CarPrefab );
        }
    }

    public void SpawnCar()
    {
        Network.Instantiate(Prefab, Vector3.zero, Quaternion.identity, 0);
    }

    protected override void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
    }
}
