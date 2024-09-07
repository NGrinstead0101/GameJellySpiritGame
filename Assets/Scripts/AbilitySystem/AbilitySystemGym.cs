using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySystemGym : MonoBehaviour
{
    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CastAbility(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CastAbility(1);
        }*/

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
