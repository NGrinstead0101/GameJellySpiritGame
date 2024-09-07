using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySystemGym : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchToAngel();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SwitchToDevil();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CastAbility(0);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CastAbility(1);
        }

    }

    /// <summary>
    /// Switches to Angel ability set
    /// </summary>
    private void SwitchToAngel()
    {
        AbilitySystem.Instance.OnSwitchAbility?.Invoke(AbilitySetType.Angel);
    }

    /// <summary>
    /// Switcges to Devil ability set
    /// </summary>
    private void SwitchToDevil()
    {
        AbilitySystem.Instance.OnSwitchAbility?.Invoke(AbilitySetType.Devil);
    }

    /// <summary>
    /// Casts an ability
    /// </summary>
    /// <param name="numAbility">ability index being casted</param>
    private void CastAbility(int numAbility)
    {
        AbilitySystem.Instance.OnCastAbility?.Invoke(numAbility);
    }
}
