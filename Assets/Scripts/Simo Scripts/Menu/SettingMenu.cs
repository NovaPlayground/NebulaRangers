using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingMenu : MonoBehaviour
{

    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;
    public void SetVolume(float volume) 
    {
        musicMixer.SetFloat("Music", volume);
        sfxMixer.SetFloat("SFX", volume);
    }
}
