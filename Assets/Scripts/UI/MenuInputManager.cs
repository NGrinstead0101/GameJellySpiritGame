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
    }

    /// <summary>
    /// Input function for when esc is pressed. Closes all the tab windows.
    /// </summary>
    private void EscapePerformed()
    {
        for(int i = 0; i < _tabs.Count; i++)
        {
            _tabs[i].gameObject.SetActive(false);
        }
        
    }

    private void SwapSpiritForm()
    {
        print("q r");
        //PlayerController.SwapForm
    }
}
