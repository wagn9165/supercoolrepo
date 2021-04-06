using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    AudioSource gunSound;

    // Start is called before the first frame update
    void Start()
    {
        gunSound = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        gunSound.Play();
    }

}
