using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System.Linq;
using UnityStandardAssets.Vehicles.Car;
using UnityStandardAssets.Utility;

[RequireComponent(typeof(CarController))]
public class Cockpit : NetworkBehaviour
{
    private CarController m_Car; // the car controller we want to use
    private new Rigidbody rigidbody;

    // NETWORK STUFF
    private float _syncTimeLast = 0f;
    private float _syncDelay = 0f;
    private float _syncTime = 0f;
    private Vector3 _syncPosStart = Vector3.zero;
    private Vector3 _syncPosEnd = Vector3.zero;
    private Quaternion _syncRotStart = Quaternion.identity;
    private Quaternion _syncRotEnd = Quaternion.identity;
    private bool _blinking = false;
    private WaypointContainer _circuit;
    public Waypoint currentWaypoint { get; private set; }
    public Waypoint nextWaypoint { get; private set; }

    public int Laps { get; private set; }

    private RaceController _race;
    private PlayerLabelView _label;

    [RPC]
    public void SetName(string name)
    {
        this.name = name;

        if (!IsRemoteControlled)
        {
            networkView.RPC("SetName", RPCMode.OthersBuffered, name);
        }
        else
        {
            if (_label == null)
            {
                _label = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/pref_PlayerLabel")).GetComponent<PlayerLabelView>();
                _label.cockpit = this;
            }
            _label.UpdateName();
        }
    }

    private void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody>();
        // get the car controller
        m_Car = GetComponent<CarController>();
        _circuit = GameObject.FindGameObjectWithTag("Circuit").GetComponent<WaypointContainer>();

        currentWaypoint = _circuit.waypoints.Last();
        nextWaypoint = _circuit.GetNextWaypoint(currentWaypoint);
    }

    private void Start()
    {
        _race = RaceController.Locate();
        _race.RegisterCar(networkView.owner, this);
    }

    [RPC]
    public void SetPosition(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    [RPC]
    public void SetState(bool active)
    {
        enabled = active;

        GetComponentInChildren<SpurtEmitter>().enabled = active;
        GetComponentInChildren<PaintPathRenderer>().enabled = active;
    }

    public float Progress
    {
        get
        {
            float offset = Laps / (float)_race.Laps;
            var wpDistance =  nextWaypoint.transform.position - currentWaypoint.transform.position;
            var myDistance = transform.position - currentWaypoint.transform.position;
            float progress = Vector3.Dot(wpDistance.normalized, myDistance) / wpDistance.magnitude;

            //Debug.LogFormat("Lap Progress: {0}, Checkpoint Progress: {1}, Checkpoint progress: {2}", offset,  ( (currentWaypoint.id + 1) % _circuit.waypoints.Count) / (float)_circuit.waypoints.Count,  progress);

            return Mathf.Clamp01(
                (Laps + (((currentWaypoint.id + 1) % _circuit.waypoints.Count) + progress) / (float)_circuit.waypoints.Count) / _race.Laps 
            );
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        var waypoint = collider.GetComponent<Waypoint>();
        if (waypoint)
        {
            if (IsNextWaypoint(waypoint))
            {
                currentWaypoint = waypoint;
                nextWaypoint = _circuit.GetNextWaypoint(currentWaypoint);

                if (_circuit.IsLast(waypoint)) Laps++;
            }
        }
        else if(collider.CompareTag("Road"))
        {
            hasRoadContact = true;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Road"))
        {
            hasRoadContact = false;
        }
    }
    private bool IsNextWaypoint(Waypoint waypoint)
    {
        return waypoint.id == nextWaypoint.id;
    }

    private bool _focused = true;
    private void OnApplicationFocus(bool focus)
    {
        _focused = focus;

        if (enabled)
        {
            if (_focused) OnEnable();
            else OnDisable();
        }
    }


    private void OnEnable()
    {
        if (!IsRemoteControlled)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
    }

    private void OnDisable()
    {
        if (!IsRemoteControlled)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void FixedUpdate()
    {
        if (!IsRemoteControlled)
        {
            //Debug.Log(Progress);

            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(Inverted ? -h : h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        } else {
            SyncedMovement();
        }
    }

    public bool isOnRoad = true;
    private bool hasRoadContact = true;
    public void Update()
    {
        if (!IsRemoteControlled)
        {
            if (m_Car.Wheels.Any(wheel => wheel.isGrounded))
                isOnRoad = hasRoadContact;

            //WheelHit hit;
            //if (m_Car.Wheels.First().GetGroundHit(out hit))
            //{
            //    isOnRoad = hit.collider.CompareTag("Road");
            //}

            if (Input.GetButtonDown("Respawn"))
            {
                Respawn();
            }
        }
    }

    public void Respawn()
    {
        StartCoroutine(Blink());

        //var roadMeshes = GameObject.FindGameObjectsWithTag("Road").Select(road => road.GetComponent<MeshFilter>().sharedMesh);
        //var pos = transform.position;

        //float minDistance = float.MaxValue;
        //Vector3 minPos = pos;
        //Vector3 direction = transform.forward;
        //foreach (var mesh in roadMeshes)
        //{
        //    var vertices = mesh.vertices;

        //    for (int i = 0; i < vertices.Length - 2; i++)
        //    {
        //        var dist = (vertices[i] - pos).sqrMagnitude;
        //        if (dist < minDistance)
        //        {
        //            minDistance = dist;
        //            minPos = (vertices[i] + vertices[i + 1]) / 2;
        //            direction = (vertices[i + 2] - vertices[i]).normalized;
        //        }
        //    }
        //}

        //transform.position = minPos + Vector3.up;
        //transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(direction, Vector3.up), Vector3.up);

        rigidbody.MovePosition(currentWaypoint.transform.position);
        rigidbody.MoveRotation(currentWaypoint.transform.rotation);

        rigidbody.velocity = Vector3.zero;

        foreach (var wheel in m_Car.Wheels)
        {
            wheel.brakeTorque = 0;
            wheel.motorTorque = 0;
        }
    }


    private IEnumerator Blink()
    {
        if (!_blinking)
        {
            _blinking = true;
            var renderers = GetComponentsInChildren<MeshRenderer>().Where(r => r.enabled).ToArray();
            float interval = 0.1f;

            for (int i = 0; i < 20; i++)
            {
                yield return new WaitForSeconds(interval);
                foreach (var renderer in renderers)
                {
                    renderer.enabled = !renderer.enabled;
                }
            }

            _blinking = false;
        }


    }

    protected override void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        // Outgoing sync
        Vector3 syncPos = Vector3.zero;
        Quaternion syncRot = Quaternion.identity;

        if (stream.isWriting)
        {
            syncRot = rigidbody.rotation;
            syncPos = rigidbody.position;
            stream.Serialize(ref syncPos);
            stream.Serialize(ref syncRot);
        }
        else // Incomming sync
        {
            if (!IsRemoteControlled)
            {
                Debug.Log("WAT");
            }

            stream.Serialize(ref syncPos);
            stream.Serialize(ref syncRot);

            _syncTime = 0f;
            _syncDelay = Time.time - _syncTimeLast;
            _syncTimeLast = Time.time;

            _syncPosStart = rigidbody.position;
            _syncPosEnd = syncPos;
            _syncRotStart = rigidbody.rotation;
            _syncRotEnd = syncRot;
        }
    }


    private void SyncedMovement()
    {
        _syncTime += Time.deltaTime;
        rigidbody.MovePosition(Vector3.Lerp(_syncPosStart,
                           _syncPosEnd,
                           _syncTime / _syncDelay));
        rigidbody.MoveRotation(Quaternion.Slerp(_syncRotStart,
                            _syncRotEnd,
                            _syncTime / _syncDelay));
    }




    public bool Inverted { get; set; }
}