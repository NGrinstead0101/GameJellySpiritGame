using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimController : MonoBehaviour
{
    public static PlayerAnimController Instance;

    [SerializeField] private Animator _angelAnimator;
    [SerializeField] private Animator _devilAnimator;

    [SerializeField] private Sprite _angelSprite;
    [SerializeField] private Sprite _devilSprite;

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

    private void OnEnable()
    {
        PlayerHealth.TakeDamageAction += TakeDamage;
        PlayerController.SwapForm += SwapAnimController;
    }
    private void OnDisable()
    {
        PlayerController.SwapForm -= SwapAnimController;
        PlayerHealth.TakeDamageAction -= TakeDamage;
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
        _angelAnimator = GetComponent<Animator>();
    }

    public void PlayMovingAnim()
    {
        _angelAnimator.SetBool("IsMoving", true);
    }

    public void StopMovingAnim()
    {
        _angelAnimator.SetBool("IsMoving", false);
    }

    private void TakeDamage()
    {
        _angelAnimator.SetTrigger("HurtTrigger");
        //_angelAnimator.ResetTrigger("HurtTrigger");
    }

    private void SwapAnimController(AbilitySetType newAbilitySet)
    {
        switch (newAbilitySet)
        {
            case AbilitySetType.Angel:
                {
                    GetComponent<SpriteRenderer>().sprite = _angelSprite;
                    _angelAnimator.enabled = true;
                    //_devilAnimator.enabled = false;
                    break;
                }
            case AbilitySetType.Devil:
                {
                    GetComponent<SpriteRenderer>().sprite = _devilSprite;
                    _angelAnimator.enabled = false;
                    _devilAnimator.enabled = true;
                    break;
                }
        }
    }
}
