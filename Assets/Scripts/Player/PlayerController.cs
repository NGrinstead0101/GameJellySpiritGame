/******************************************************************
 *    Author: Nick Grinstead
 *    Contributors: 
 *    Description: Handles player inputs and movement.
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // True is Angel, False is Devil
    public static Action<bool> SwapForm;

    [SerializeField] private float _jumpSpeed;
    [SerializeField] private float _baseSpeed;
    [SerializeField] private float _angelSpeedModifier;
    [SerializeField] private float _devilSpeedModifier;

    private Rigidbody2D _rb;
    private float _horizVelocity;
    private bool _canJump = true;
    private bool _jumpQueued = false;

    private bool _currentForm = true;

    private GameplayInputs _gamePlayInputs;
    private InputAction _move, _jump, _swapToAngel, _swapToDevil;
    private float _moveDirection = 0f;

    /// <summary>
    /// Sets up inputs
    /// </summary>
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        _gamePlayInputs = new GameplayInputs();
        _gamePlayInputs.Enable();

        _move = _gamePlayInputs.FindAction("Move");
        _jump = _gamePlayInputs.FindAction("Jump");
        _swapToAngel = _gamePlayInputs.FindAction("SwapAngel");
        _swapToDevil = _gamePlayInputs.FindAction("SwapDevil");

        _move.performed += ctx => _moveDirection = _move.ReadValue<float>();
        _move.canceled += ctx => _moveDirection = _move.ReadValue<float>();
        _jump.performed += ctx => Jump();
        _swapToAngel.performed += ctx => SwapSpiritForm();
        _swapToDevil.performed += ctx => SwapSpiritForm();
    }

    /// <summary>
    /// Unregisters input callbacks
    /// </summary>
    private void OnDisable()
    {
        _gamePlayInputs.Disable();
        _move.performed -= ctx => _moveDirection = _move.ReadValue<float>();
        _move.canceled -= ctx => _moveDirection = _move.ReadValue<float>();
        _jump.performed -= ctx => Jump();
        _swapToAngel.performed -= ctx => SwapSpiritForm();
        _swapToDevil.performed -= ctx => SwapSpiritForm();
    }

    /// <summary>
    /// Updates rigidbody velocity
    /// </summary>
    private void FixedUpdate()
    {
        // Determine horizontal velocity
        if (_currentForm)
        {
            _horizVelocity = _baseSpeed * _angelSpeedModifier * _moveDirection;
        }
        else
        {
            _horizVelocity = _baseSpeed * _devilSpeedModifier * _moveDirection;
        }

        // Apply velocity to rigidbody
        if (_jumpQueued)
        {
            _jumpQueued = false;
            float modifiedJumpSpeed = _currentForm ? _jumpSpeed * _angelSpeedModifier : _jumpSpeed * _devilSpeedModifier;
            _rb.velocity = new Vector2(_horizVelocity, modifiedJumpSpeed);
        }
        else
        {
            _rb.velocity = new Vector2(_horizVelocity, _rb.velocity.y);
        }
    }

    /// <summary>
    /// Updates current form and invokes action
    /// </summary>
    private void SwapSpiritForm()
    {
        _currentForm = !_currentForm;
        SwapForm?.Invoke(_currentForm);
    }

    /// <summary>
    /// Applies horizontal movement to player
    /// </summary>
    private void UpdateHorizVelocity()
    {
        if (_currentForm)
        {
            _horizVelocity = _baseSpeed * _angelSpeedModifier * _moveDirection;
        }
        else
        {
            _horizVelocity = _baseSpeed * _devilSpeedModifier * _moveDirection;
        }
    }

    /// <summary>
    /// Applies vertical force to player when jumping
    /// </summary>
    private void Jump()
    {
        if (_canJump)
        {
            _canJump = false;
            _jumpQueued = true;
        }
    }

    /// <summary>
    /// Updates player ability to jump
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            _canJump = true;
        }
    }
}
