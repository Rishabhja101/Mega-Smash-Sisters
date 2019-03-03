using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventAudio : MonoBehaviour
{

    public AudioClip sound;
    public AudioSource audioS;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void PlaySound()
    {
        audioS.PlayOneShot(sound);
    }
}
