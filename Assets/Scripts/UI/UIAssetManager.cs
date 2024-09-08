/******************************************************************
 *    Author: Marissa 
 *    Contributors: 
 *    Description: Handles the game's different assets for angel/devil.
 *    There should be one in every scene.
 *******************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAssetManager : MonoBehaviour
{
    [SerializeField] private List<AssetReference> _assetReferences = new List<AssetReference>();

    public static Action<AbilitySetType> SwitchAssets;
    public static Action<bool> BlackFade;

    [System.Serializable] public struct AssetReference
    {
        public string assetName;
        public GameObject _gameObjectReference;
        [HideInInspector] public Image _image;
        public Sprite _angelVersion;
        public Sprite _devilVersion;
    }

    private void OnEnable()
    {
        PlayerController.SwapForm += UpdateUIAssets;
        SwitchAssets += UpdateUIAssets;
    }
    private void OnDisable()
    {
        PlayerController.SwapForm -= UpdateUIAssets;
        SwitchAssets -= UpdateUIAssets;
    }

    /// <summary>
    /// Goes through the list of assets and switches to the correct type.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="type"></param>
    private void UpdateUIAssets(AbilitySetType type)
    {
        if(type == AbilitySetType.Angel)
        {
            for (int i = 0; i < _assetReferences.Count; i++)
            {
                //ensures asset's Image field is set
                if (_assetReferences[i]._image == null)
                {
                    var copyAsset = _assetReferences[i];
                    copyAsset._image = _assetReferences[i]._gameObjectReference.GetComponent<Image>();
                    _assetReferences[i] = copyAsset;
                }

                //sets asset sprite
                _assetReferences[i]._image.sprite = _assetReferences[i]._angelVersion;
            }
        }
        else
        {
            for (int i = 0; i < _assetReferences.Count - 1; i++)
            {
                //ensures asset's Image field is set
                if (_assetReferences[i]._image == null)
                {
                    var copyAsset = _assetReferences[i];
                    copyAsset._image = _assetReferences[i]._gameObjectReference.GetComponent<Image>();
                    _assetReferences[i] = copyAsset;
                }

                //sets asset sprite
                _assetReferences[i]._image.sprite = _assetReferences[i]._devilVersion;
            }
        }
    }
}
