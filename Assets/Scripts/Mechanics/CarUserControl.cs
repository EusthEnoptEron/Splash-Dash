using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using System.Linq;
using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(CarController))]
public class CarUserControl : NetworkBehaviour
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

    private void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody>();
        // get the car controller
        m_Car = GetComponent<CarController>();

        if (!IsRemoteControlled)
        {
            
        }
    }


    private void FixedUpdate()
    {
        if (!IsRemoteControlled)
        {
            // Disable mouse
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            

            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        } else {
            SyncedMovement();
        }
    }

    public void Update()
    {
        if (!IsRemoteControlled)
        {
            if (Input.GetButtonDown("Respawn"))
            {
                Respawn();
            }
        }
    }
    private void Respawn()
    {
        StartCoroutine(Blink());

        var roadMeshes = GameObject.FindGameObjectsWithTag("Road").Select(road => road.GetComponent<MeshFilter>().sharedMesh);
        var pos = transform.position;

        float minDistance = float.MaxValue;
        Vector3 minPos = pos;
        Vector3 direction = transform.forward;
        foreach (var mesh in roadMeshes)
        {
            var vertices = mesh.vertices;

            for (int i = 0; i < vertices.Length - 2; i++)
            {
                var dist = (vertices[i] - pos).sqrMagnitude;
                if (dist < minDistance)
                {
                    minDistance = dist;
                    minPos = (vertices[i] + vertices[i + 1]) / 2;
                    direction = (vertices[i + 2] - vertices[i]).normalized;
                }
            }
        }

        transform.position = minPos + Vector3.up;
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(direction, Vector3.up), Vector3.up);
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



}