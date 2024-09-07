/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Description: Handles the game's scene transitions, music changes,
 *      and assets for angel/devil.
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private UIAssetManager _assetManager;
    private CanvasBehavior _canvasBehavior;

    public static GameManager Instance { get; private set; }

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
