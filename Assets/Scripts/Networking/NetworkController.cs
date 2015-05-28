using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkController : MonoBehaviour {
    private const string TYPE_NAME = "BFH.GameDev.SplashDash";
    private const string GAME_NAME = "Splash Dash";
    private HostData[] hostList;
    private RaceController race;

    public int initialSplashes = 500;
    public Vector2 initialSplashSize = new Vector2(1f, 20f);
    public Color[] initialSplashColors = new Color[] {
        Color.green,
        Color.red,
        Color.blue
    };

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

        var brush = paintbrush.GetComponent<PaintBrush>();

        var mapSize = Ruler.Measure();

        for (int i = 0; i < initialSplashes; i++)
        {
            brush.Paint(
                Random.insideUnitSphere * mapSize.x / 2,
                initialSplashColors[Random.Range(0, initialSplashColors.Length)], 
                Random.Range(
                    (int)(initialSplashSize.x * PaintBrush.SCALE_FACTOR), 
                    (int)(initialSplashSize.y * PaintBrush.SCALE_FACTOR)
                )
            );
        }
    }

    private void SpawnPlayer()
    {
        var myCar = Network.Instantiate(carPrefab, transform.position, transform.rotation, 0) as GameObject;
        var myCockpit = myCar.GetComponent<Cockpit>();
        myCockpit.SetName("Player #" + Random.Range(1, 1000));
    }

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Clean up after player " + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }
}
