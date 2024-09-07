using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Ability", menuName = "Abilities/NewAbility", order = 0)]
public class AbilityInformation : ScriptableObject
{
    #region Art Assets

    #endregion

    #region Ability Info

    [SerializeField] private string _abilityName;

    #endregion
}
