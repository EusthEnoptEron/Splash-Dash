using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public abstract class Pickup : NetworkBehaviour {
    public AudioClip pickupSound;
    public AudioMixerGroup outputAudioMixerGroup;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<NetworkView>().isMine)
        {
            var trunk = other.GetComponentInParent<CarTrunk>();
            if (trunk != null)
            {
                // PICKUP!
                if (PickUp(trunk))
                {
                    Destroy();
                }
            }
        }
    }

    private void PlayOneShot(AudioClip clip)
    {
        var oneshot = new GameObject("Audio One Shot").AddComponent<AudioSource>();
        oneshot.transform.position = transform.position;
        oneshot.outputAudioMixerGroup = outputAudioMixerGroup;
        oneshot.loop = false;
        oneshot.playOnAwake = false;
        oneshot.clip = clip;

        oneshot.Play();

        GameObject.Destroy(oneshot.gameObject, clip.length);
    }

    private void Destroy()
    {
        networkView.RPC("DestroyRPC",  RPCMode.All);
    }

    [RPC]
    private void DestroyRPC()
    {
        if (pickupSound)
        {
            PlayOneShot(pickupSound);
        }

        // we got picked up
        Network.Destroy(gameObject);
    }

    protected abstract bool PickUp(CarTrunk trunk);
}
