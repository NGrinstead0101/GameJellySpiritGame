using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Heal : Ability
{
    [SerializeField] private int _healthIncrease;
    [SerializeField] private float _timeBetweenHeal;

    [Header("Optional Settings")]
    [Tooltip("Uncheck this if you want to cap how much the player can heal while using the ability")]
    [SerializeField] private bool _infiniteHealing = true;

    [Tooltip("Max amount of health that can be increased if infinite healing is disabled")]
    [SerializeField] private int _maxHealthIncrease = 0;

    // Amount player has healed so far. Only tracked if infinite healing is disabled
    private int _amountHealed = 0;

    private bool _isHealing = false;

    private AngelAnimations _animationController;

    protected override void Initialize()
    {
        base.Initialize();
        _animationController = AngelAnimations.Instance;
    }

    public override void CastAbility()
    {
        if (_canCast)
        {
            // Ability should not activate if the player is at max healt
            if (PlayerHealth.Instance.CurrentHealth < PlayerHealth.Instance.MaxHealth)
            {
                if (_animationController != null)
                    _animationController.SetClickTrigger("LeftClick");

                Debug.Log("CastHealing");
                base.CastAbility();
                StartCoroutine(StartHealing());
                SfxManager.Instance.PlaySFX("Healing");
            }
        }
        else
        {
            if (_animationController != null)
                _animationController.SetClickTrigger("LeftClickInvalid");
        }
    }

    /// <summary>
    /// Stops healing and cancels the ability
    /// </summary>
    public override void CancelAbility()
    {
        SfxManager.Instance.StopSFX("Healing");

        if (_isHealing)
        {
            Debug.Log("CancelHealing");
            base.CancelAbility();
            _isHealing = false;
        }
    }

    /// <summary>
    /// Begins player healing
    /// </summary>
    private IEnumerator StartHealing()
    {
        _isHealing = true;
        _amountHealed = 0;

        while (_isHealing)
        {
            PlayerHealth.Instance.HealPlayer(_healthIncrease);

            // Cancel heal if player is at maximum health
            if (PlayerHealth.Instance.CurrentHealth >= PlayerHealth.Instance.MaxHealth)
            {
                CancelAbility();
            }

            /* If player can infinitely heal, move past the rest of the code.*/
            if (_infiniteHealing)
            {
                yield return new WaitForSeconds(_timeBetweenHeal);
                continue;
            }
            /* If the player healing is capped, increment the amount healed and exit when the max heal has
             * been exceeded */
            _amountHealed += _healthIncrease;
            if (_amountHealed >= _maxHealthIncrease)
            {
                CancelAbility();
            }

            yield return new WaitForSeconds(_timeBetweenHeal);
        }
        StartCoroutine(AbilityCooldown());
    }
}
