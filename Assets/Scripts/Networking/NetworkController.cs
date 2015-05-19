using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkController : MonoBehaviour {
    private const string TYPE_NAME = "BFH.GameDev.SplashDash";
    private const string GAME_NAME = "Splash Dash";
    private HostData[] hostList;
    private RaceController race;


    public GameObject carPrefab;
    public GameObject paintbrushPrefab;
    public static bool IsConnected
    {
        get
        {
            return Network.isClient || Network.isServer;
        }
    }

    public Dictionary<NetworkPlayer, SplashPlayer> players = new Dictionary<NetworkPlayer, SplashPlayer>();

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
        if (!IsConnected)
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
        else if (Network.isServer && race.State == RaceState.Preparing)
        {
            if (GUILayout.Button("Start Game"))
            {
                race.StartRace();
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
        var paintbrush = Network.Instantiate(paintbrushPrefab, Vector3.zero, Quaternion.identity, 0) as GameObject;
        paintbrush.transform.SetParent(transform, false);

        race = paintbrush.GetComponent<RaceController>();

        SpawnPlayer();

    }

    private void SpawnPlayer()
    {
        var myCar = Network.Instantiate(carPrefab, transform.position, transform.rotation, 0) as GameObject;
        Camera.main.GetComponent<SmoothFollow>().target = myCar.transform;
    }

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Clean up after player " + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }
}
