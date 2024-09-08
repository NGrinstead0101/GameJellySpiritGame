/******************************************************************
 *    Author: Nick Grinstead
 *    Contributors: 
 *    Description: Handles player inputs and movement. Also updates
 *      fog of war light as player form changes.
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public static Action<AbilitySetType> SwapForm;
    public static Action JumpAction;

    [SerializeField] private float _jumpSpeed;
    [SerializeField] private float _baseSpeed;
    [SerializeField] private float _angelSpeedModifier;
    [SerializeField] private float _devilSpeedModifier;
    [SerializeField] private float _angelLightDistance;
    [SerializeField] private float _devilLightDistance;
    [SerializeField] private float _lightTransitionTime;

    private Light2D _characterLight;
    private Rigidbody2D _rb;
    private float _horizVelocity;
    private bool _canJump = true;
    private bool _jumpQueued = false;

    private bool _currentForm = true;

    public static GameplayInputs GameplayInputs;
    private InputAction _move, _jump, _swapToAngel, _swapToDevil, _pause;
    private float _moveDirection = 0f;

    /// <summary>
    /// Sets up inputs
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        _rb = GetComponent<Rigidbody2D>();
        _characterLight = GetComponent<Light2D>();
        _characterLight.pointLightOuterRadius = _currentForm ? _angelLightDistance : _devilLightDistance;

        GameplayInputs = new GameplayInputs();
        GameplayInputs.Enable();

        _move = GameplayInputs.FindAction("Move");
        _jump = GameplayInputs.FindAction("Jump");
        _swapToAngel = GameplayInputs.FindAction("SwapAngel");
        _swapToDevil = GameplayInputs.FindAction("SwapDevil");
        _pause = GameplayInputs.FindAction("Escape");

        AbilitySystem.BindUseAbility();
        PlayerAnimController.Instance.BindAnims();

        _move.performed += ctx => _moveDirection = _move.ReadValue<float>();
        _move.canceled += ctx => _moveDirection = _move.ReadValue<float>();

        _jump.performed += ctx => Jump();
        _swapToAngel.performed += ctx => SwapSpiritForm();
        _swapToDevil.performed += ctx => SwapSpiritForm();
        _pause.performed += ctx => PausePerformed();
    }


    private void Start()
    {
        //set player spirit form
        if (GameManager.Instance.GetCurrentAbilityType() == AbilitySetType.Devil)
        {
            _currentForm = false;
            SwapForm?.Invoke(AbilitySetType.Devil);
        }
        else
        {
            _currentForm = true;
            SwapForm?.Invoke(AbilitySetType.Angel);
        }
    }

    /// <summary>
    /// Unregisters input callbacks
    /// </summary>
    private void OnDisable()
    {
        GameplayInputs.Disable();
        _move.performed -= ctx => _moveDirection = _move.ReadValue<float>();
        _move.canceled -= ctx => _moveDirection = _move.ReadValue<float>();
        _jump.performed -= ctx => Jump();
        _swapToAngel.performed -= ctx => SwapSpiritForm();
        _swapToDevil.performed -= ctx => SwapSpiritForm();

        _move.performed -= ctx => PlayerAnimController.Instance.PlayMovingAnim();
        _move.canceled -= ctx => PlayerAnimController.Instance.StopMovingAnim();
        _pause.performed -= ctx => PausePerformed();
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

        // Flips player attack hitbox
        if (_horizVelocity < 0)
        {
            transform.localScale = new Vector2(-(Math.Abs(transform.localScale.x)), transform.localScale.y);
        }
        else if (_horizVelocity > 0)
        {
            transform.localScale = new Vector2(Math.Abs(transform.localScale.x), transform.localScale.y);
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
        if(_currentForm)
        {
            SwapForm?.Invoke(AbilitySetType.Devil);
        }
        else
        {
            SwapForm?.Invoke(AbilitySetType.Angel);
        }

        _currentForm = !_currentForm;
        StopAllCoroutines();
        StartCoroutine(nameof(LerpLightDistance));        
    }

    /// <summary>
    /// Smoothly transitions between different lighting states
    /// </summary>
    private IEnumerator LerpLightDistance()
    {
        float startingDistance = _characterLight.pointLightOuterRadius;
        float targetDistance = _currentForm ? _angelLightDistance : _devilLightDistance;
        float timeWaited = 0f;

        while (timeWaited < _lightTransitionTime)
        {
            yield return new WaitForSeconds(0.05f);

            timeWaited += 0.05f;

            _characterLight.pointLightOuterRadius = 
                Mathf.Lerp(startingDistance, targetDistance, timeWaited / _lightTransitionTime);
        }

        _characterLight.pointLightOuterRadius = _currentForm ? _angelLightDistance : _devilLightDistance;
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

    private void Update()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, 1.75f, 1 << LayerMask.NameToLayer("Wall")))
        {        
            _canJump = true;
        }
        else
        {
            _canJump = false;
        }
    }

        /// <summary>
        /// Applies vertical force to player when jumping
        /// </summary>
        private void Jump()
    {
        if (_canJump)
        {
            JumpAction?.Invoke();
            _canJump = false;
            _jumpQueued = true;
        }
    }

    /// <summary>
    /// Updates player ability to jump
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("END"))
        {
            GameManager.Instance.LoadEnding();
        }
    }

    /// <summary>
    /// Coroutine to fade the player's sprite as the "go through the door". Then calls
    /// the game manager to load the next level.
    /// </summary>
    /// <returns></returns>
    public IEnumerator EnterDoorTransition()
    {
        GameplayInputs.Disable();

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        while (sr.color.a > 0)
        {
            Color tmp = sr.color;
            tmp.a -= 0.05f;
            sr.color = tmp;
            yield return new WaitForSeconds(0.05f);
        }

        GameManager.Instance.ChangeGameState(GameManager.GameState.level);
    }

    /// <summary>
    /// Function called by pause input.
    /// </summary>
    private void PausePerformed()
    {
        GameManager.Instance.PauseInput();
    }
}
