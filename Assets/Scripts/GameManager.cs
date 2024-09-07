/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Description: Handles the game's scene transitions, music changes,
 *      and assets for angel/devil.
 *******************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private UIAssetManager _assetManager;
    private CanvasBehavior _canvasBehavior;

    private bool _angelTutComplete;
    private bool _devilTutComplete;
    [SerializeField] int _numberOfLevels;

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

    public GameState GetCurrentGameState()
    {
        return _currentGameState;
    }

    /// <summary>
    /// Changes the state of the game. 
    /// </summary>
    /// <param name="state"></param> state to change to
    public void ChangeGameState(GameState state)
    {
        switch(state)
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
        //fade out of current scene
        //load main menu scene, will fade in on its own
        //change state
    }

    /// <summary>
    /// Enables the pause menu on level's canvas
    /// </summary>
    private void LoadPauseMenu()
    {
        //already in level scene
        //switch pause menu art
        //find canvas and call fx there
    }

    /// <summary>
    /// Loads the first level based on the current player type
    /// </summary>
    private void LoadFirstLevel()
    {
        //if curent player state is angel
        //load angel tutorial scene
        //vice versa
    }

    /// <summary>
    /// Disables the pause menu and resumes the game
    /// </summary>
    private void ReturnToLevel()
    {
        //leaves the pause menu
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
                LoadScene(2);
            }
        }
        //if in devil tutorial scene
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            _devilTutComplete = true;
            if (!_angelTutComplete)
            {
                //load angel scene
                LoadScene(1);
            }
        }
        //both tutorials completed
        else
        {
            //if in one of the tutorial levels
            if(SceneManager.GetActiveScene().buildIndex < 3)
            {
                LoadScene(3);
            }
            //if is in final level
            else if(SceneManager.GetActiveScene().buildIndex >= _numberOfLevels)
            {
                //TODO: Any final transitions?
                LoadMainMenu();
            }
            //in any other level
            else
            {
                LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

    }
    
    /// <summary>
    /// fades out of the current scene and then loads the given scene
    /// </summary>
    /// <param name="index"></param>
    private void LoadScene(int index)
    {
        //fade current scene
        SceneManager.LoadScene(index);
    }
#endregion

    /// <summary>
    /// Finds the asset manager in each scene
    /// </summary>
    private void FindManagers()
    {
        GameObject asset = GameObject.FindWithTag("AssetManager");
        _assetManager = asset.GetComponent<UIAssetManager>();

        GameObject canvas = GameObject.FindWithTag("Canvas");
        _canvasBehavior = canvas.GetComponent<CanvasBehavior>();
    }
}
