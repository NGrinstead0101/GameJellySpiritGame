using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class BackgroundMusicManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _musicMixer;

    [SerializeField] private GameObject _levelMusic;
    private AudioSource _levelMusicSource;
    [SerializeField] private GameObject _pauseMusic;
    private AudioSource _pauseMusicSource;
    [SerializeField] private GameObject _menuMusic;
    private AudioSource _menuMusicSource;

    [SerializeField] private GameObject _angelMusic;
    private AudioSource _angelMusicSource;
    [SerializeField] private GameObject _devilMusic;
    private AudioSource _devilMusicSource;

    [SerializeField] private float _angelDevilTransitionTime;
    [SerializeField] private float _backTrackTransitionTime;
    private bool _playingAngel;

    public static BackgroundMusicManager Instance { get; private set; }

    public enum BackTrack
    {
        menu,
        pause,
        level
    }

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
    }

    private void Start()
    {
        //get audio source components
        _levelMusicSource = _levelMusic.GetComponent<AudioSource>();
        _pauseMusicSource = _pauseMusic.GetComponent<AudioSource>();
        _menuMusicSource = _menuMusic.GetComponent<AudioSource>();
        _angelMusicSource = _angelMusic.GetComponent<AudioSource>();
        _devilMusicSource = _devilMusic.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        //TODO: assign SwitchMusic fx += ability switch action
    }

    private void OnDisable()
    {
        //TODO: assign SwitchMusic fx -= ability switch action
    }


    #region angel/devil
    /// <summary>
    /// switched the devil/angel dynamic tracks
    /// </summary>
    private void SwitchMusic()
    {
        _playingAngel = !_playingAngel;

        if(_playingAngel )
        {
            StartCoroutine(StartFade(_devilMusicSource, 0, _angelDevilTransitionTime));
            StartCoroutine(StartFade(_angelMusicSource, 1, _angelDevilTransitionTime));
        }
        else
        {
            StartCoroutine(StartFade(_devilMusicSource, 1, _angelDevilTransitionTime));
            StartCoroutine(StartFade(_angelMusicSource, 0, _angelDevilTransitionTime));
        }
    }

    /// <summary>
    /// Coroutine that fades the angel and devil tracks in and out 
    /// </summary>
    /// <param name="audioSource"></param> angel or devil audio source
    /// <param name="targetVolume"></param> 1 for on and 0 for off
    /// <returns></returns>
    public IEnumerator StartFade(AudioSource audioSource, float targetVolume, float duration)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
    #endregion

    #region selecting back track

    public void SwitchBackTrack(BackTrack from, BackTrack to)
    {
        StartCoroutine(StartFade(EnumToClip(from), 0, _backTrackTransitionTime));
        StartCoroutine(StartFade(EnumToClip(to), 1, _backTrackTransitionTime));
    }

    private AudioSource EnumToClip(BackTrack input)
    {
        switch(input)
        {
            case BackTrack.menu:
                return _menuMusicSource;
            case BackTrack.pause:
                return _pauseMusicSource;
            case BackTrack.level:
                return _levelMusicSource;
            default:
                return null;
        }
    }

    #endregion

    #region music sfx group methods for options menu

    //TODO: music group fade in and out for options menu

    #endregion

}
