using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimController : MonoBehaviour
{
    public static PlayerAnimController Instance;

    private Animator _animator;

    private static InputAction _move = null;

    /// <summary>
    /// Binds the move ability action to GameplayInputs. Called in PlayerController awake
    /// </summary>
    public void BindAnims()
    {
        _move = PlayerController.GameplayInputs.FindAction("Move");

        _move.performed += ctx => PlayMovingAnim();
        _move.canceled += ctx => StopMovingAnim();
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayMovingAnim()
    {
        _animator.SetBool("IsMoving", true);
    }

    public void StopMovingAnim()
    {
        _animator.SetBool("IsMoving", false);
    }
}
