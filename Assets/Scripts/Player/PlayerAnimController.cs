using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.InputSystem;

public class PlayerAnimController : MonoBehaviour
{
    public static PlayerAnimController Instance;

    [Header("Animation Controllers")]
    [SerializeField] private UnityEditor.Animations.AnimatorController _angelAnimController;
    [SerializeField] private UnityEditor.Animations.AnimatorController _devilAnimController;
    private Animator _animator = null;

    [Header("Sprite VFX")]
    [SerializeField] private Sprite _angelSprite;
    [SerializeField] private Sprite _devilSprite;

    [Header("Death VFX")]
    [SerializeField] private GameObject _angelDeathVFXObject;
    [SerializeField] private GameObject _devilDeathVFXObject;
    private ParticleSystem _angelDeathVFX;
    private ParticleSystem _devilDeathVFX;
    private ParticleSystem _currentDeathVFX;

    [Header("Hit VFX")]
    [SerializeField] private GameObject _angelHitVFXObject;
    [SerializeField] private GameObject _devilHitVFXObject;
    private ParticleSystem _angelHitVFX;
    private ParticleSystem _devilHitVFX;
    private ParticleSystem _currentHitVFX = null;

    [Header("Transform VFX")]
    [SerializeField] private GameObject _transformVFXGameObject;
    private ParticleSystem _transformVFX;

    private static InputAction _move = null;
    private static InputAction _jump = null;

    /// <summary>
    /// Binds the move ability action to GameplayInputs. Called in PlayerController awake
    /// </summary>
    public void BindAnims()
    {
        _move = PlayerController.GameplayInputs.FindAction("Move");
        _jump = PlayerController.GameplayInputs.FindAction("Jump");

        _move.performed += ctx => PlayMovingAnim();
        _move.canceled += ctx => StopMovingAnim();

        
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _angelDeathVFX = _angelDeathVFXObject.GetComponent<ParticleSystem>();
        _devilDeathVFX = _devilDeathVFXObject.GetComponent<ParticleSystem>();

        _angelHitVFX = _angelHitVFXObject.GetComponent<ParticleSystem>();
        _devilHitVFX = _devilHitVFXObject.GetComponent<ParticleSystem>();

        _transformVFX = _transformVFXGameObject.GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        PlayerHealth.TakeDamageAction += TakeDamage;
        PlayerController.SwapForm += SwapAnimController;
        PlayerHealth.DeathAction += PlayDeathVFX;
        Ability_LungeAttack.Lunge += Attack;
        PlayerController.JumpAction += Jump;
    }
    private void OnDisable()
    {
        PlayerController.SwapForm -= SwapAnimController;
        PlayerHealth.TakeDamageAction -= TakeDamage;
        PlayerHealth.DeathAction -= PlayDeathVFX;
        Ability_LungeAttack.Lunge -= Attack;
        PlayerController.JumpAction -= Jump;

        if (_move != null)
        {
            _move.performed -= ctx => PlayMovingAnim();
            _move.canceled -= ctx => StopMovingAnim();
        }

        if (_jump != null)
        {
            _jump.performed -= ctx => Jump();
        }
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

    public void PlayMovingAnim()
    {
        _animator.SetBool("IsMoving", true);
    }

    public void StopMovingAnim()
    {
        _animator.SetBool("IsMoving", false);
    }

    private void TakeDamage()
    {
        _animator.SetTrigger("Hurt");
        PlayHitVFX();
    }

    public void Jump()
    {
        _animator.SetTrigger("Jump");
    }

    public void Attack()
    {
        _animator.SetTrigger("Attack");
    }

    private void SwapAnimController(AbilitySetType newAbilitySet)
    {
        PlaySwapVFX();
        switch (newAbilitySet)
        {
            case AbilitySetType.Angel:
                {
                    GetComponent<SpriteRenderer>().sprite = _angelSprite;
                    _animator.runtimeAnimatorController = _angelAnimController;
                    _currentHitVFX = _angelHitVFX;
                    _currentDeathVFX = _angelDeathVFX;
                    /*_angelAnimator.enabled = true;
                    _devilAnimator.enabled = false;*/
                    break;
                }
            case AbilitySetType.Devil:
                {
                    GetComponent<SpriteRenderer>().sprite = _devilSprite;
                    _animator.runtimeAnimatorController = _devilAnimController;
                    _currentDeathVFX = _devilDeathVFX;
                    _currentHitVFX = _devilHitVFX;
                    /*_angelAnimator.enabled = false;
                    _devilAnimator.enabled = true;*/
                    break;
                }
        }
    }

    private void PlayHitVFX()
    {
        _currentHitVFX.Play();
    }

    private void PlayDeathVFX()
    { 
        _currentDeathVFX.Play();
    }

    private void PlaySwapVFX()
    {
        _transformVFX.Play();
    }
}
