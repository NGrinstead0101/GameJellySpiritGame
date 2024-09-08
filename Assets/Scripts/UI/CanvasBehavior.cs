using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBehavior : MonoBehaviour
{
    [SerializeField] private GameObject _fadeToBlack;
    [SerializeField] private GameObject _pauseMenu;
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        if(GameManager.Instance.GetCurrentGameState() != GameManager.GameState.menu)
        {
            UIAssetManager.SwitchAssets?.Invoke(GameManager.Instance.GetCurrentAbilityType());
        }
    }

    private void OnEnable()
    {
        UIAssetManager.BlackFade += TriggerBlackFadeAnim;
        GameManager.PauseAction += SetPauseMenu;
    }


    private void OnDisable()
    {
        UIAssetManager.BlackFade -= TriggerBlackFadeAnim;
        GameManager.PauseAction -= SetPauseMenu;
    }

    /// <summary>
    /// Sets the fade to black image to on/off, 1/0. Called by animation event
    /// </summary>
    /// <param name="input"></param> om = 1, off = 0
    public void EnableFTB(int input)
    {
        if(input == 1) 
        {
            _fadeToBlack.SetActive(true);
        }
        else
        {
            _fadeToBlack.SetActive(false);
        }
    }

    /// <summary>
    /// Triggers the fade to/from black animation. 
    /// </summary>
    /// <param name="input"></param> true = fade to, false = fade from
    public void TriggerBlackFadeAnim(bool input)
    {
        if(input)
        {
            _anim.SetTrigger("FadeToBlack");
        }
        else
        {
            _anim.SetTrigger("FadeFromBlack");
        }
    }

    private void SetPauseMenu(bool input)
    {
        if(input)
        {
            _pauseMenu.SetActive(true);
        }
        else
        {
            _pauseMenu.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        GameManager.Instance.ChangeGameState(GameManager.GameState.level);
    }

    public void LoadFirstLevel()
    {
        GameManager.Instance.ChangeGameState(GameManager.GameState.level);
    }
}
