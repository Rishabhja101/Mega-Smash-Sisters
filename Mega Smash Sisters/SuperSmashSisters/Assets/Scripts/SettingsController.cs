using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    private AudioSource backgroundMusicSlider;
    private AudioSource soundEffectSlider;
    private Slider BackgroundMusicSlider;
    private Slider SoundEffectSlider;
    public AudioSource[] soundEffectSliders;

    // Use this for initialization
    void Start ()
    {
        backgroundMusicSlider = GameObject.Find("/BackgroundMusic").GetComponent<AudioSource>();
        soundEffectSlider = GameObject.Find("/SoundEffect").GetComponent<AudioSource>();
        BackgroundMusicSlider = GameObject.Find("BackgroundMusicSlider").GetComponent<Slider>();
        SoundEffectSlider = GameObject.Find("SoundEffectSlider").GetComponent<Slider>();
        backgroundMusicSlider.volume = Global.backgroundMusicVolume;
        soundEffectSlider.volume = Global.soundEffectsVolume;
        BackgroundMusicSlider.value = Global.backgroundMusicVolume;
        SoundEffectSlider.value = Global.soundEffectsVolume;
        for (int i = 0; i < soundEffectSliders.Length; i++)
        {
            soundEffectSliders[i].volume = Global.soundEffectsVolume;
        }
    }

    public void setBackgroundVolume()
    {
        Global.backgroundMusicVolume = backgroundMusicSlider.volume;
    }

    public void setSoundEffectVolume()
    {
        Global.soundEffectsVolume = soundEffectSlider.volume;
    }
}
