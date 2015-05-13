using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public abstract class NetworkBehaviour : MonoBehaviour {
    protected NetworkView networkView;

    protected void Awake()
    {
        networkView = GetComponent<NetworkView>();
        networkView.observed = this;
    }

    public bool IsRemoteControlled
    {
        get
        {
            return networkView && networkView.enabled && !networkView.isMine;
        }
    }

    protected abstract void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info);

}
