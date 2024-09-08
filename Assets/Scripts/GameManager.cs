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

    private AbilitySetType _currentAbilityType;

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
            DontDestroyOnLoad(Instance);
        }

    }

    private void Start()
    {
        _numberOfLevels = SceneManager.sceneCountInBuildSettings - 1;

        UIAssetManager.BlackFade?.Invoke(false);
    }

    private void OnEnable()
    {
        PlayerController.SwapForm += SetCurrentAbilityType;
    }
    private void OnDisable()
    {
        PlayerController.SwapForm -= SetCurrentAbilityType;
    }

    public GameState GetCurrentGameState()
    {
        return _currentGameState;
    }

    public AbilitySetType GetCurrentAbilityType()
    {
        return _currentAbilityType;
    }

    private void SetCurrentAbilityType(AbilitySetType type)
    {
        _currentAbilityType = type;
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
        LoadScene(0);
        _currentGameState = GameState.menu;
        //load menu art for the main menu
    }

    /// <summary>
    /// Enables the pause menu on level's canvas
    /// </summary>
    private void LoadPauseMenu()
    {
        //already in level scene
        //switch pause menu art
        //find canvas and call fx there
        //switch state
        //switch music
    }

    /// <summary>
    /// Disables the pause menu and resumes the game
    /// </summary>
    private void ReturnToLevel()
    {
        //leaves the pause menu
    }

    /// <summary>
    /// Loads the first level based on the current player type
    /// </summary>
    private void LoadFirstLevel()
    {
        if(_currentAbilityType == AbilitySetType.Angel)
        {
            //load angel scene
            StartCoroutine(LoadScene(1));
        }
        else
        {
            //load devil scene
            StartCoroutine(LoadScene(2));
        }
    }

    /// <summary>
    /// Loads the next level with scene transition
    /// </summary>
    private void LoadNextLevel()
    {
        //if in angel tutorial scene
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            _angelTutComplete = true;
            if(!_devilTutComplete)
            {
                //load devil scene
                StartCoroutine(LoadScene(2));
            }
        }
        //if in devil tutorial scene
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            _devilTutComplete = true;
            if (!_angelTutComplete)
            {
                //load angel scene
                StartCoroutine(LoadScene(1));
            }
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
        UIAssetManager.BlackFade?.Invoke(true);

        //sets avility type for tutorial levels
        if (index == 1)
        {
            SetCurrentAbilityType(AbilitySetType.Angel);
        }
        else if (index == 2)
        {
            SetCurrentAbilityType(AbilitySetType.Devil);
        }

        yield return new WaitForSeconds(_sceneTransitionTime);

        SceneManager.LoadScene(index);

        yield return new WaitForSeconds(_sceneTransitionTime);

        UIAssetManager.BlackFade?.Invoke(false);
    }

    //private void Update()
    //{
    //    if(Input.GetKeyUp(KeyCode.P))
    //    {
    //        StartCoroutine(LoadScene(1));
    //    }
    //}

    #endregion
}
