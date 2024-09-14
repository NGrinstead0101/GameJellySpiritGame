using System;
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

    
    [SerializeField] private GameObject _angelMenuGO;
    AudioSource _angelMenu;

    [SerializeField] private GameObject _angelLevelGO;
    AudioSource _angelLevel;

    [SerializeField] private GameObject _angelPauseGo;
    AudioSource _angelPause;

    [SerializeField] private GameObject _devilMenuGO;
    AudioSource _devilMenu;

    [SerializeField] private GameObject _devilLevelGO;
    AudioSource _devilLevel;

    [SerializeField] private GameObject _devilPauseGO;
    AudioSource _devilPause;

    [SerializeField] private List<AudioSource> _angelDevilClips = new List<AudioSource>();

    public static BackgroundMusicManager Instance { get; private set; }

    

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

        _angelMenu = _angelMenuGO.GetComponent<AudioSource>();
        _angelLevel = _angelLevelGO.GetComponent<AudioSource>();
        _angelPause = _angelPauseGo.GetComponent<AudioSource>();
        _devilMenu = _devilMenuGO.GetComponent<AudioSource>();
        _devilLevel = _devilMenuGO.GetComponent<AudioSource>();
        _devilPause = _devilMenuGO.GetComponent<AudioSource>();

        SwitchBackTrackLoad(GameManager.GameState.level, GameManager.GameState.menu);
    }

    private void OnEnable()
    {
        PlayerController.SwapForm += SwitchDynamicMusic;
    }

    private void OnDisable()
    {
        PlayerController.SwapForm -= SwitchDynamicMusic;
    }


    #region angel/devil
    /// <summary>
    /// switched the devil/angel dynamic tracks
    /// </summary>
    private void SwitchDynamicMusic(AbilitySetType form)
    {
        if (form == AbilitySetType.Angel)
        {
            switch (GameManager.Instance.GetCurrentGameState())
            {
                case GameManager.GameState.menu:
                    //angel menu on
                    StartCoroutine(StartFade(_angelMenu, 1, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_devilMenu, 0, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_angelLevel, 0, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_angelPause, 0, _angelDevilTransitionTime));
                    break;
                case GameManager.GameState.level:
                    //angel level on
                    StartCoroutine(StartFade(_angelMenu, 0, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_devilLevel, 0, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_angelLevel, 1, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_angelPause, 0, _angelDevilTransitionTime));
                    break;
                case GameManager.GameState.pause:
                    //angel pause on
                    StartCoroutine(StartFade(_angelMenu, 0, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_devilPause, 0, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_angelLevel, 0, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_angelPause, 1, _angelDevilTransitionTime));
                    break;
            }
        }
        else
        {
            switch (GameManager.Instance.GetCurrentGameState())
            {
                case GameManager.GameState.menu:
                    //devil menu on
                    print(GameManager.Instance.GetCurrentGameState());
                    print(GameManager.Instance.GetCurrentAbilityType());
                    StartCoroutine(StartFade(_angelMenu, 0, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_devilMenu, 1, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_devilLevel, 0, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_devilPause, 0, _angelDevilTransitionTime));
                    break;
                case GameManager.GameState.level:
                    //devil level on
                    StartCoroutine(StartFade(_devilLevel, 1, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_devilMenu, 0, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_angelLevel, 0, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_devilPause, 0, _angelDevilTransitionTime));
                    break;
                case GameManager.GameState.pause:
                    //devil pause on
                    StartCoroutine(StartFade(_devilLevel, 0, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_devilMenu, 0, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_angelPause, 0, _angelDevilTransitionTime));
                    StartCoroutine(StartFade(_devilPause, 1, _angelDevilTransitionTime));
                    break;
            }
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

    public void SwitchBackTrack(GameManager.GameState from, GameManager.GameState to)
    {
        StartCoroutine(StartFade(EnumToClip(from), 0, _backTrackTransitionTime));
        StartCoroutine(StartFade(EnumToClip(to), 1, _backTrackTransitionTime));

        SwitchDynamicMusic(GameManager.Instance.GetCurrentAbilityType());
    }

    public void SwitchBackTrackLoad(GameManager.GameState from, GameManager.GameState to)
    {
        StartCoroutine(StartFade(EnumToClip(from), 0, _backTrackTransitionTime));
        StartCoroutine(StartFade(EnumToClip(to), 1, _backTrackTransitionTime));
    }

    private AudioSource EnumToClip(GameManager.GameState input)
    {
        switch(input)
        {
            case GameManager.GameState.menu:
                return _menuMusicSource;
            case GameManager.GameState.pause:
                return _pauseMusicSource;
            case GameManager.GameState.level:
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
