/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Description: Sfx manager singleton. Call to start a sound effect
 *******************************************************************/
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;
using System.Collections;

public class SfxManager : MonoBehaviour
{
    [SerializeField] private List<SFX> _SFXs = new List<SFX>();
    [SerializeField] private float _fadeInDuration;
    [SerializeField] private float _fadeOutDuration;
    public static SfxManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }

        //create audio components and set fields
        for(int i = 0;  i < _SFXs.Count; i++)
        {
            //need to be different game objects so sounds can overlap
            GameObject go = Instantiate(new GameObject(), transform);

            _SFXs[i].source = go.AddComponent<AudioSource>();
            _SFXs[i].source.outputAudioMixerGroup = _SFXs[i].mixer;
            _SFXs[i].source.volume = _SFXs[i].maxVolume;
            _SFXs[i].source.pitch = _SFXs[i].pitch;
            _SFXs[i].source.playOnAwake = false;
            _SFXs[i].source.loop = _SFXs[i].doLoop;
        }
    }

    /// <summary>
    /// Plays the given sound effect. Finds the index of the specific sound effect,
    /// sets the audio clip based on the avaliable clips, and then plays the clip
    /// </summary>
    /// <param name="name"></param>
    public void PlaySFX(string name)
    {
        SFX sfx = _SFXs[_SFXs.FindIndex(i => i.name == name)];
        sfx.source.clip = sfx.clips[UnityEngine.Random.Range(0, sfx.clips.Length)];
        sfx.source.Play();
    }

    /// <summary>
    /// Stops the given sound effect from playing. Finds the index of the specific
    /// sound effect, and then stops the clip.
    /// </summary>
    /// <param name="name"></param>
    public void StopSFX(string name)
    {
        SFX sfx = _SFXs[_SFXs.FindIndex(i => i.name == name)];
        sfx.source.Stop();
    }

    public void FadeInSFX(string name)
    {
        //play sound
        SFX sfx = _SFXs[_SFXs.FindIndex(i => i.name == name)];
        sfx.source.clip = sfx.clips[UnityEngine.Random.Range(0, sfx.clips.Length)];
        sfx.source.volume = 0;
        sfx.source.Play();

        StartCoroutine(StartFade(sfx.source, sfx.maxVolume, _fadeInDuration));
    }

    public void FadeOutSFX(string name)
    {
        //find sound
        SFX sfx = _SFXs[_SFXs.FindIndex(i => i.name == name)];
        
        StartCoroutine(StartFade(sfx.source, 0, _fadeOutDuration));
    }

    private IEnumerator StartFade(AudioSource audioSource, float targetVolume, float duration)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        if(targetVolume <= 0)
        {
            audioSource.Stop();
        }

        yield break;
    }
}
