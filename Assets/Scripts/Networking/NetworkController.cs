using UnityEngine;
using System.Collections;

public class NetworkController : MonoBehaviour {
    private const string TYPE_NAME = "BFH.GameDev.SplashDash";
    private const string GAME_NAME = "Splash Dash";
    private HostData[] hostList;


    public GameObject carPrefab;

	// Use this for initialization
    private void Start()
    {
 
    }

    public void StartServer()
    {
        Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(TYPE_NAME, GAME_NAME);
    }

    void OnGUI()
    {
        if (!Network.isClient && !Network.isServer)
        {
            if (GUILayout.Button("Start Server"))
                StartServer();
            if (GUILayout.Button("Refresh Hosts"))
                MasterServer.RequestHostList(TYPE_NAME);
            if (hostList != null)
            {
                for (int i = 0; i < hostList.Length; i++)
                {
                    if (GUILayout.Button(hostList[i].gameName))
                        Network.Connect(hostList[i]);
                }
            }
        }
    }
    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
            hostList = MasterServer.PollHostList();
    }
    void OnConnectedToServer()
    {
        SpawnPlayer();
    }

    void OnServerInitialized()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var myCar = Network.Instantiate(carPrefab, transform.position, transform.rotation, 0) as GameObject;
        Camera.main.GetComponent<SmoothFollow>().target = myCar.transform;
    }
}
