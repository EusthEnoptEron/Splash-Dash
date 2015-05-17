using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public abstract class Pickup : MonoBehaviour {
    public AudioClip pickupSound;
    public AudioMixerGroup outputAudioMixerGroup;

    void OnTriggerEnter(Collider other)
    {
        var trunk = other.GetComponentInParent<CarTrunk>();
        if (trunk != null)
        {
            // PICKUP!
            if (PickUp(trunk))
            {
                if (pickupSound)
                {
                    PlayOneShot(pickupSound);
                }

                // we got picked up
                Destroy(gameObject);
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
    protected abstract bool PickUp(CarTrunk trunk);
}
