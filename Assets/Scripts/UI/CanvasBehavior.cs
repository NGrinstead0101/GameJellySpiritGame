using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CanvasBehavior : MonoBehaviour
{
    [SerializeField] private GameObject _fadeToBlack;
    [SerializeField] private GameObject _pauseMenu;
    private Animator _anim;

    private bool _hasStarted = false;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _hasStarted = false;
        if(GameManager.Instance.GetCurrentGameState() != GameManager.GameState.menu)
        {
            UIAssetManager.SwitchAssets?.Invoke(GameManager.ActiveAbilitySetType);
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
        if(_pauseMenu != null)
        {
            if (input)
            {
                _pauseMenu.SetActive(true);
            }
            else
            {
                _pauseMenu.SetActive(false);
            }
        }
    }

    public void ResumeGame()
    {
        if(!_hasStarted)
        {
            _hasStarted = true;
            GameManager.Instance.ChangeGameState(GameManager.GameState.level);    
        }    
    }

    public void LoadFirstLevel()
    {
        GameManager.Instance.ChangeGameState(GameManager.GameState.level);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        _hasStarted = false;
        GameManager.Instance.ChangeGameState(GameManager.GameState.menu);
    }

    public void PlayUISound()
    {

        if (GameManager.ActiveAbilitySetType == AbilitySetType.Angel)
        {
            SfxManager.Instance.PlaySFX("MenuUIAngel");
        }
        else
        {
            SfxManager.Instance.PlaySFX("MenuUIDevil");
        }
    }

    public void  ResetLevel()
    {
        GameManager.Instance.ResetCurrentScene();
    }
}
