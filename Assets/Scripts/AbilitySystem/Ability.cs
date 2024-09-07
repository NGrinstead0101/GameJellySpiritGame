using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    [SerializeField] protected AbilityInformation _abilityInformation;

    protected float _cooldown = 0.0f;


    private void Start()
    {
        _cooldown = _abilityInformation.Cooldown;
    }

    /// <summary>
    /// In-Game Mechanic effect of the ability.
    /// </summary>
    public virtual void CastAbility()
    {
        //Debug.Log(_abilityInformation.name + " Parent Casted");
    }
}
