using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    [SerializeField] protected AbilityInformation _abilityInformation;

    protected float _cooldown = 0.0f;

    protected bool _canCast = true;

    private void Start()
    {
        _cooldown = _abilityInformation.Cooldown;
    }

    /// <summary>
    /// In-Game Mechanic effect of the ability.
    /// </summary>
    public virtual void CastAbility()
    {
        if (_canCast)
        {
            _canCast = false;
            //Debug.Log(_abilityInformation.name + " Parent Casted");
        }
    }

    public virtual void CancelAbility()
    {
        //Debug.Log(_abilityInformation.name + " Parent Cancelled");
    }

    protected IEnumerator AbilityCooldown()
    {
        yield return new WaitForSeconds(_cooldown);
        _canCast = true;
    }
}
