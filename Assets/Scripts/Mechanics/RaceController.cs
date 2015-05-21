﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;
using System.Collections.Generic;
using System.Linq;


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
    public int Laps = 3;

    private Animator GUI;

    private Transform[] startPositions = new Transform[0];

    //private List<CarController> _cars = new List<CarController>();
    private List<KeyValuePair<NetworkPlayer, CarUserControl>> _cars = new List<KeyValuePair<NetworkPlayer, CarUserControl>>();

    private List<CarUserControl> _racingCars = new List<CarUserControl>();
    private List<NetworkPlayer> _topList = new List<NetworkPlayer>();


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
	
	}

    public bool IsFull
    {
        get
        {
            return _cars.Count >= startPositions.Length;
        }
    }

    internal void RegisterCar(NetworkPlayer player, CarUserControl car)
    {
        if (State == RaceState.Preparing)
        {
            if (!IsFull)
            {
                car.SetState(false);

                //_cars.Add(new KeyValuePair< car);
                _cars.Add(new KeyValuePair<NetworkPlayer, CarUserControl>(player, car));
                _racingCars.Add(car);

                if (Network.isServer)
                {
                    car.GetComponent<NetworkView>().RPC("SetPosition", RPCMode.AllBuffered, startPositions[_cars.Count - 1].position, startPositions[_cars.Count - 1].rotation);
                    //car.transform.position = startPositions[_cars.Count - 1].position;
                    //car.transform.rotation = startPositions[_cars.Count - 1].rotation;
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

    private IEnumerator RaceCoroutine()
    {


        State = RaceState.Starting;

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
                    _cars.RemoveAt(i);
                }
                else
                {
                    Debug.LogFormat("{0}: {1}", car.name, car.Laps);
                    if (car.Laps >= Laps)
                    {
                        _cars.RemoveAt(i);
                        _topList.Add(car.GetComponent<NetworkView>().owner);
                    }
                }

            }
            yield return null;
        }

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
}
