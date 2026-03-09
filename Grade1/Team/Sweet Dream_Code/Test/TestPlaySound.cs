using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlaySound : MonoBehaviour
{
    public AudioSource PlayAudio;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayAudio.Play();
        }
    }
}
