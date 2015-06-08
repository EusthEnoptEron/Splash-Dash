using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public enum RaceState
{
    Preparing,
    Starting,
    Running,
    Ended
}
    
public class RaceController : NetworkBehaviour {
    public GameObject prefDigit;
    public GameObject GUIPrefab;
    private GameObject prefRaceUI;


    public int Laps = 1;

    private Animator GUI;

    private Transform[] startPositions = new Transform[0];

    //private List<CarController> _cars = new List<CarController>();
    private List<KeyValuePair<NetworkPlayer, Cockpit>> _cars = new List<KeyValuePair<NetworkPlayer, Cockpit>>();

    private List<Cockpit> _racingCars = new List<Cockpit>();
    private List<Cockpit> _topList = new List<Cockpit>();

    private List<Cockpit> _positionTable = new List<Cockpit>();

    private bool _focused = true;
    private bool _paused;

    public RaceState State
    {
        get;
        private set;
    }

    public static RaceController Locate()
    {
        return GameObject.FindObjectOfType<RaceController>();
    }

	// Use this for initialization
	protected override void Awake () {
        base.Awake();

        prefRaceUI = Resources.Load<GameObject>("Prefabs/pref_RaceUI");

        networkView.stateSynchronization = NetworkStateSynchronization.Off;

        State = RaceState.Preparing;

        var startPositionsContainer = GameObject.FindGameObjectWithTag("Start Positions");
        if (startPositionsContainer != null)
        {
            startPositions = new Transform[startPositionsContainer.transform.childCount].Select((t, i) => startPositionsContainer.transform.GetChild(i)).ToArray();
        }
        else
        {
            Debug.LogError("Start positions not found!");
        }

        GUI = GameObject.Instantiate<GameObject>(GUIPrefab).GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        UpdateCursor();
	}

    void UpdateCursor()
    {
        Cursor.lockState = (_focused && (State == RaceState.Running || State == RaceState.Starting) && !_paused) ? CursorLockMode.Confined : CursorLockMode.None;
        Cursor.visible = _focused && (State == RaceState.Running || State == RaceState.Starting) && !_paused ? false : true;
    }

    private void SyncPositionTable()
    {
        if (Network.isServer)
        {

            // Make table
            
            var newTable = _topList.Concat(_racingCars).ToList();
            if(!newTable.SequenceEqual( _positionTable )) {
                Debug.Log("SYNC SENT");

                _positionTable = newTable;

                // Sync with others
                using(var stream = new MemoryStream()) {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, _positionTable.Select(c => c.name).ToArray() );

                    networkView.RPC("SyncPositionTableRPC", RPCMode.Others, stream.ToArray());
                }
            }
            
        }
    }

    public void SetPaused(bool value)
    {
        _paused = value;
        MyCar.SetState(!value);
    }

    private void OnApplicationFocus(bool focus)
    {
        _focused = focus;
    }




    public bool IsFull
    {
        get
        {
            return _cars.Count >= startPositions.Length;
        }
    }

    internal void RegisterCar(NetworkPlayer player, Cockpit car)
    {
        if (State == RaceState.Preparing)
        {
            if (!IsFull)
            {
                car.SetState(false);

                //_cars.Add(new KeyValuePair< car);
                _cars.Add(new KeyValuePair<NetworkPlayer, Cockpit>(player, car));
                _racingCars.Add(car);

                if (Network.isServer)
                {
                    car.GetComponent<NetworkView>().RPC("SetPosition", RPCMode.AllBuffered, startPositions[_cars.Count - 1].position, startPositions[_cars.Count - 1].rotation);
                    //car.transform.position = startPositions[_cars.Count - 1].position;
                    //car.transform.rotation = startPositions[_cars.Count - 1].rotation;
                }

                if(!car.IsRemoteControlled) {
                    MyCar = car;
                }
            }
            else
            {
                Debug.LogError("Already full!");
            }
        }
        else
        {
            Debug.LogError("You ain't gonna join a started game, freak!");
            Network.Destroy(car.gameObject);
        }
    }

    public void UnregisterCar(NetworkPlayer player)
    {

    }

    private IEnumerator RaceCoroutine()
    {
        // change to car!
        Camera.main.GetComponent<FreeCamera>().enabled = false;
        Camera.main.GetComponent<SmoothFollow>().enabled = true;
        Camera.main.GetComponent<SmoothFollow>().target = MyCar.transform;



        State = RaceState.Starting;

        var raceUI = GameObject.Instantiate<GameObject>(prefRaceUI);
        SyncPositionTable();
       
        yield return StartCoroutine(DoCountdown());

        State = RaceState.Running;

        if (Network.isServer)
        {
            foreach (var car in _cars)
            {
                car.Value.GetComponent<NetworkView>().RPC("SetState", RPCMode.AllBuffered, true);
                //car.Value.enabled = true;
            }
        }

        // Racin' the shit out of this game
        while (_racingCars.Count > 0)
        {
            for (int i = _racingCars.Count - 1; i >= 0; i--)
            {
                var car = _racingCars[i];
                if (!car)
                {
                    // User was apparently deleted
                    _racingCars.RemoveAt(i);
                }
                else
                {
                    if (car.Laps >= Laps)
                    {
                        _racingCars.RemoveAt(i);
                        _topList.Add(car);

                        car.SetState(false);
                    }
                }
            }

            _racingCars.Sort( (lhs, rhs) => {
                float p1 = lhs.Progress;
                float p2 = rhs.Progress;

                if (p1 < p2)
                    return 1;
                if (p1 == p2)
                    return 0;
                return -1;
            });

            SyncPositionTable();

            yield return null;
        }

        EndRace();
    }

    private IEnumerator DoCountdown()
    {
        GUI.SetTrigger("Start");

        yield return new WaitForSeconds(2);

        SendText("3", Color.red);

        yield return new WaitForSeconds(1);

        SendText("2", Color.green);

        yield return new WaitForSeconds(1);

        SendText("1", Color.blue);

        yield return new WaitForSeconds(1);

        SendText("GO!", Color.white);
        GUI.SetBool("Done", true);

        yield return new WaitForSeconds(1);


    }

    private void SendText(string text, Color color)
    {
        var digit = GameObject.Instantiate<GameObject>(prefDigit);
        digit.transform.SetParent(GUI.transform, false);

        var digitText = digit.GetComponentInChildren<Text>();

        digitText.text = text;
        digitText.color = color;

        var outline = digitText.GetComponent<Outline>();
        if (outline)
        {
            outline.effectColor = color - new Color(0.3f, 0.3f, 0.3f);
        }
    }

    protected override void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {

    }

    public Cockpit MyCar { get; private set; }

    public int GetRank(Cockpit cockpit)
    {
        return _positionTable.IndexOf(cockpit) + 1;
    }



    #region RPC functions


    [RPC]
    private void SyncPositionTableRPC(byte[] serializedTable)
    {
        Debug.Log("SYNC RECEIVED");
        var formatter = new BinaryFormatter();

        using (var stream = new MemoryStream(serializedTable))
        {
            var list = formatter.Deserialize(stream) as string[];

            _positionTable = list.Select(
                cName => _cars.FirstOrDefault(playerCar => playerCar.Value.name == cName).Value
            ).ToList();
        }
    }

    [RPC]
    public void StartRace()
    {

        if (State == RaceState.Preparing)
        {
            StartCoroutine(RaceCoroutine());
        }

        if (Network.isServer)
        {
            networkView.RPC("StartRace", RPCMode.Others);
        }
    }


    [RPC]
    public void EndRace()
    {
        State = RaceState.Ended;
        foreach (var playerCar in _cars)
        {
            if (playerCar.Value)
            {
                playerCar.Value.SetState(false);
            }
        }

        if (Network.isServer) networkView.RPC("EndRace", RPCMode.OthersBuffered);

        var rankingEl = GameObject.Find("Ranking").transform;
        var rankingItem = Resources.Load<GameObject>("Prefabs/pref_RankingEntry");
           
        // Show ranking
        for (int i = 0; i < _positionTable.Count; i++)
        {
            int position = i + 1;
            string name = _positionTable[i].name;

            var item = GameObject.Instantiate<GameObject>(rankingItem);
            item.transform.SetParent(rankingEl, false);

            item.transform.FindChild("Position").GetComponent<Text>().text = position.ToSuffixedString();
            item.transform.FindChild("Name").GetComponent<Text>().text = name;
        }

    }

    #endregion
}
