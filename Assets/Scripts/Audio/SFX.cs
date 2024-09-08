/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Description: Sxf class
 *******************************************************************/
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SFX 
{
    public string name;

    public AudioClip[] clips;

    [HideInInspector]
    public AudioClip clip;

    public AudioMixerGroup mixer;

    [Range(0f, 1f)]
    public float maxVolume;

    [Range(0f, 3f)]
    public float pitch;

    public bool doLoop;

    [HideInInspector]
    public AudioSource source;
}
