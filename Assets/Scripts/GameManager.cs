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

    public static GameManager Instance { get; private set; }

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
    private void FindAssetManager()
    {
        GameObject go = GameObject.FindWithTag("AssetManager");
        _assetManager = go.GetComponent<UIAssetManager>();
    }
}
