using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInputManager : MonoBehaviour
{
    public static GameplayInputs GameplayInputs;
    private InputAction _swapToAngel, _swapToDevil, _escape;

    [SerializeField] private List<GameObject> _tabs;
    [SerializeField] private GameObject _continueButton;
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
            _continueButton.SetActive(true);

            if (GameManager.Instance.GetCurrentAbilityType() == AbilitySetType.Angel)
            {
                PlayerController.SwapForm?.Invoke(AbilitySetType.Devil);
            }
            else
            {
                PlayerController.SwapForm?.Invoke(AbilitySetType.Angel);
            }
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
