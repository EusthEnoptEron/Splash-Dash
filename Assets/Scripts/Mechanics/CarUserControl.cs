using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

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


    private void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody>();
        // get the car controller
        m_Car = GetComponent<CarController>();
    }


    private void FixedUpdate()
    {
        if (!IsRemoteControlled)
        {
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