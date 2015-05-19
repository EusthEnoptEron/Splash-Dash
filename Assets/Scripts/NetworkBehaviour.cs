using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public abstract class NetworkBehaviour : MonoBehaviour {
    protected new NetworkView networkView;

    protected virtual void Awake()
    {
        networkView = GetComponent<NetworkView>();
        networkView.observed = this;
    }

    public bool IsRemoteControlled
    {
        get
        {
            return NetworkController.IsConnected && networkView && networkView.enabled && !networkView.isMine;
        }
    }

    protected virtual void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) { }

}
