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
    [SerializeField] private List<AssetSpriteReference> _assetSpriteReferences = new List<AssetSpriteReference>();
    [SerializeField] private List<AssetGameObjectReference> _assetGameObjectReferences = new List<AssetGameObjectReference>();

    public static Action<AbilitySetType> SwitchAssets;
    public static Action<bool> BlackFade;

    [System.Serializable] public struct AssetSpriteReference
    {
        public string assetName;
        public GameObject _gameObjectReference;
        [HideInInspector] public Image _image;
        public Sprite _angelVersion;
        public Sprite _devilVersion;
    }

    [System.Serializable]
    public struct AssetGameObjectReference
    {
        public string assetName;
        public GameObject _angelVersion;
        public GameObject _devilVersion;

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
        print("Asset manager");
        if(type == AbilitySetType.Angel)
        {
            for (int i = 0; i < _assetSpriteReferences.Count; i++)
            {
                //ensures asset's Image field is set
                if (_assetSpriteReferences[i]._image == null)
                {
                    var copyAsset = _assetSpriteReferences[i];
                    copyAsset._image = _assetSpriteReferences[i]._gameObjectReference.GetComponent<Image>();
                    _assetSpriteReferences[i] = copyAsset;
                }

                //sets asset sprite
                _assetSpriteReferences[i]._image.sprite = _assetSpriteReferences[i]._angelVersion;
            }

            for (int i = 0; i < _assetGameObjectReferences.Count; i++)
            {
                //sets game object
                _assetGameObjectReferences[i]._angelVersion.SetActive(true);
                _assetGameObjectReferences[i]._devilVersion.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < _assetSpriteReferences.Count; i++)
            {
                //ensures asset's Image field is set
                if (_assetSpriteReferences[i]._image == null)
                {
                    var copyAsset = _assetSpriteReferences[i];
                    copyAsset._image = _assetSpriteReferences[i]._gameObjectReference.GetComponent<Image>();
                    _assetSpriteReferences[i] = copyAsset;
                }

                //sets asset sprite
                _assetSpriteReferences[i]._image.sprite = _assetSpriteReferences[i]._devilVersion;
            }

            for (int i = 0; i < _assetGameObjectReferences.Count; i++)
            {
                //sets game object
                _assetGameObjectReferences[i]._angelVersion.SetActive(false);
                _assetGameObjectReferences[i]._devilVersion.SetActive(true);
            }
        }
    }
}
