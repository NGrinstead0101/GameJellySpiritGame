/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Description: Handles the game's scene transitions, music changes,
 *      and assets for angel/devil.
 *******************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private UIAssetManager _assetManager;
    private CanvasBehavior _canvasBehavior;

    private bool _angelTutComplete;
    private bool _devilTutComplete;
    int _numberOfLevels;
    [SerializeField] float _sceneTransitionTime;

    public static Action<bool> PauseAction;

    public static AbilitySetType ActiveAbilitySetType = AbilitySetType.Angel;

    public static GameManager Instance { get; private set; }

    private GameState _currentGameState;
    public enum GameState
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
            
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        _numberOfLevels = SceneManager.sceneCountInBuildSettings - 1;
    }

    public GameState GetCurrentGameState()
    {
        return _currentGameState;
    }

    /// <summary>
    /// Changes the state of the game and loads scenes accordingly
    /// </summary>
    /// <param name="state"></param> state to change to
    public void ChangeGameState(GameState state)
    {
        switch (state)
        {
            case GameState.menu:
                LoadMainMenu();
                break;
            case GameState.pause:
                LoadPauseMenu();
                break;
            case GameState.level:
                if (_currentGameState == GameState.menu)
                {
                    LoadFirstLevel();
                }
                else if(_currentGameState == GameState.pause)
                {
                    ReturnToLevel();
                }
                else
                {
                    LoadNextLevel();
                }
                break;
        }
    }

    #region state helper methods

    /// <summary>
    /// Loads the main menu scene
    /// </summary>
    private void LoadMainMenu()
    {
        if(_currentGameState == GameState.pause)
        {
            //_currentGameState = GameState.level;
            Time.timeScale = 1;
            BackgroundMusicManager.Instance.SwitchBackTrack(GameState.pause, GameState.menu);
        }
        LoadScene(0);
        _currentGameState = GameState.menu;
        BackgroundMusicManager.Instance.RestartBackTracks();

        BackgroundMusicManager.Instance.SwitchBackTrack(GameState.level, GameState.menu);
    }

    /// <summary>
    /// Called from the player controller when input is detected.
    /// </summary>
    public void PauseInput()
    {
        if (_currentGameState == GameState.level)
        {
            ChangeGameState(GameState.pause);
        }
        else
        {
            ChangeGameState(GameState.level);
        }
    }

    /// <summary>
    /// Enables the pause menu on level's canvas
    /// </summary>
    private void LoadPauseMenu()
    {
        _currentGameState = GameState.pause;
        Time.timeScale = 0;
        PauseAction.Invoke(true);
        BackgroundMusicManager.Instance.SwitchBackTrack(GameState.level, GameState.pause);
        //switch music
    }

    /// <summary>
    /// Disables the pause menu and resumes the game
    /// </summary>
    private void ReturnToLevel()
    {
        _currentGameState = GameState.level;
        Time.timeScale = 1;
        PauseAction.Invoke(false);
        BackgroundMusicManager.Instance.SwitchBackTrack(GameState.pause, GameState.level);
    }

    /// <summary>
    /// Loads the first level based on the current player type
    /// </summary>
    private void LoadFirstLevel()
    {
        if(GameManager.ActiveAbilitySetType == AbilitySetType.Angel)
        {
            StartCoroutine(LoadScene(1));
        }
        else
        {
            //load devil scene
            StartCoroutine(LoadScene(2));
        }

        _currentGameState = GameState.level;
        BackgroundMusicManager.Instance.SwitchBackTrack(GameState.menu, GameState.level);
        BackgroundMusicManager.Instance.RestartBackTracks();
    }

    /// <summary>
    /// Loads the next level with scene transition
    /// </summary>
    private void LoadNextLevel()
    {
        //print(SceneManager.GetActiveScene().buildIndex);
        //if in angel tutorial scene
        if(SceneManager.GetActiveScene().buildIndex == 1 && !_devilTutComplete)
        {
            _angelTutComplete = true;
            StartCoroutine(LoadScene(2));
        }
        //if in devil tutorial scene
        else if (SceneManager.GetActiveScene().buildIndex == 2 & !_angelTutComplete)
        {
            _devilTutComplete = true;
            //load angel scene
            StartCoroutine(LoadScene(1));
        }
        //both tutorials completed
        else
        {
            //if in one of the tutorial levels
            if(SceneManager.GetActiveScene().buildIndex < 3)
            {
                StartCoroutine(LoadScene(3));
            }
            //if is in final level
            else if(SceneManager.GetActiveScene().buildIndex >= _numberOfLevels)
            {
                //TODO: Any final transitions or fun stuff?
                LoadMainMenu();
            }
            //in any other level
            else
            {
                StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
            }
        }

    }
    
    /// <summary>
    /// fades out of the current scene and then loads the given scene
    /// </summary>
    /// <param name="index"></param>
    private IEnumerator LoadScene(int index)
    {
        Time.timeScale = 1;
        UIAssetManager.BlackFade?.Invoke(true);

        //sets ability type for tutorial levels
        if (index == 1)
        {
            PlayerController.SwapForm?.Invoke(AbilitySetType.Angel);
        }
        else if (index == 2)
        {
            PlayerController.SwapForm?.Invoke(AbilitySetType.Devil);
        }

        yield return new WaitForSeconds(_sceneTransitionTime);

        SceneManager.LoadScene(index);

        yield return new WaitForSeconds(_sceneTransitionTime);

        UIAssetManager.BlackFade?.Invoke(false);
    }

    public void ResetCurrentScene()
    {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    public void LoadEnding()
    {
        StartCoroutine(LoadScene(10));
        StartCoroutine(Return());
    }

    private IEnumerator Return()
    {
        yield return new WaitForSeconds(10f);
        LoadMainMenu();
    }    

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            //StartCoroutine(LoadScene(1));
            _currentGameState = GameState.level;
        }
    }

    #endregion
}
