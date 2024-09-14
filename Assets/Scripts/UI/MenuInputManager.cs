using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInputManager : MonoBehaviour
{
    public static Action<AbilitySetType> SwapFormMenu;
    public static GameplayInputs GameplayInputs;
    private InputAction _swapToAngel, _swapToDevil, _escape;

    [SerializeField] private List<GameObject> _tabs;
    [SerializeField] private GameObject _continueButtonD;
    [SerializeField] private GameObject _continueButtonA;
    private bool _canSwap;

    private void Awake()
    {
        GameplayInputs = new GameplayInputs();
        GameplayInputs.Enable();

        _swapToAngel = GameplayInputs.FindAction("SwapAngel");
        _swapToDevil = GameplayInputs.FindAction("SwapDevil");
        _escape = GameplayInputs.FindAction("Escape");

        _swapToAngel.performed += ctx => SwapSpiritForm();
        _swapToDevil.performed += ctx => SwapSpiritForm();
        _escape.performed += ctx => EscapePerformed();

        _canSwap = true;
    }

    private void OnDisable()
    {
        GameplayInputs.Disable();
        _swapToAngel.performed -= ctx => SwapSpiritForm();
        _swapToDevil.performed -= ctx => SwapSpiritForm();
        _escape.performed -= ctx => EscapePerformed();
    }

    /// <summary>
    /// Input function for when esc is pressed. Closes all the tab windows.
    /// </summary>
    private void EscapePerformed()
    {
        for (int i = 0; i < _tabs.Count; i++)
        {
            _tabs[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Swaps menu assets
    /// </summary>
    private void SwapSpiritForm()
    {
        if (_canSwap)
        {
            if (GameManager.ActiveAbilitySetType == AbilitySetType.Angel)
            {
                GameManager.ActiveAbilitySetType = AbilitySetType.Devil;
                SfxManager.Instance.PlaySFX("DevilMenuSwitch");
            }
            else
            {
                GameManager.ActiveAbilitySetType = AbilitySetType.Angel;
                SfxManager.Instance.PlaySFX("AngelMenuSwitch");
            }
            UIAssetManager.SwitchAssets?.Invoke(GameManager.ActiveAbilitySetType);
            SwapFormMenu?.Invoke(GameManager.ActiveAbilitySetType);
        }
    }

    /// <summary>
    /// called by the continue button. 
    /// </summary>
    public void StopSwap()
    {
        _canSwap = false;
    }

}
