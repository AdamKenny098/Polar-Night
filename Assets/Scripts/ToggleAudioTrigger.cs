using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAudioTrigger : MonoBehaviour
{
    public AudioSource targetAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && targetAudio)
        {
            targetAudio.mute = false;
        }

        if (other.CompareTag("Player") && !GameManager.Instance.hasGivenStarterFuel)
        {
            FuelStorage.Instance.AddStartingFuel(20);
            GameManager.Instance.hasGivenStarterFuel = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && targetAudio)
        {
            targetAudio.mute = true;
        }
    }
}
