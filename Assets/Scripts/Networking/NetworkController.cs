using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkController : MonoBehaviour {
    private const string GAME_NAME = "Splash Dash";
    private HostData[] hostList;
    private RaceController race;
    public GameObject startServerCanvas;

    public int initialSplashes = 500;
    public Vector2 initialSplashSize = new Vector2(1f, 20f);
    public Color[] initialSplashColors = new Color[] {
        Color.green,
        Color.red,
        Color.blue
    };

    public GameObject carPrefab;
    public GameObject paintbrushPrefab;
    private BirdsEye minimapCamera;

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
        minimapCamera = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/pref_Minimap")).GetComponent<BirdsEye>();

        if (GamePresets.IsSlave)
        {
            if (GamePresets.Host != null)
            {
                Network.Connect(GamePresets.Host);
            }
            else
            {
                Debug.LogError("No host given!");
                Application.LoadLevel("Lobby");
            }
        }
        else
        {
            StartServer();

            startServerCanvas.SetActive(true);
        }
    }

    public void StartServer()
    {
        string sname = GamePresets.ServerName;

        if (sname == "" || sname == null)
            sname = "Untitled server #" + Random.Range(1, 10000);
        Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(GamePresets.TYPE_NAME, GamePresets.ServerName);
    }

   
    public void StartRace()
    {
        if (Network.isServer)
        {
            startServerCanvas.SetActive(false);
            MasterServer.UnregisterHost();
            race.StartRace();
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
        var myCar = Network.Instantiate( Resources.Load<GameObject>("Prefabs/Cars/pref_Car" + (GamePresets.CarNo + 1)), transform.position, transform.rotation, 0) as GameObject;
        var myCockpit = myCar.GetComponent<Cockpit>();

        minimapCamera.target = myCar.transform;

        myCockpit.SetName(GamePresets.PlayerName);
    }

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Clean up after player " + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }
}
