using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    [SerializeField] protected AbilityInformation _abilityInformation;

    [SerializeField] protected GameObject _vfxObject;
    protected ParticleSystem _vfxParticleSystem;

    protected float _cooldown = 0.0f;

    protected bool _canCast = true;

    private void Start()
    {
        _cooldown = _abilityInformation.Cooldown;
        _vfxParticleSystem = _vfxObject.GetComponent<ParticleSystem>();

        SetParticleSystem(false);
    }

    /// <summary>
    /// In-Game Mechanic effect of the ability.
    /// </summary>
    public virtual void CastAbility()
    {
        if (_canCast)
        {
            _canCast = false;
            SetParticleSystem(true);
            //Debug.Log(_abilityInformation.name + " Parent Casted");
        }
    }

    public virtual void CancelAbility()
    {
        //Debug.Log(_abilityInformation.name + " Parent Cancelled");
        SetParticleSystem(false);
    }

    protected IEnumerator AbilityCooldown()
    {
        yield return new WaitForSeconds(_cooldown);
        _canCast = true;
    }

    protected virtual void SetParticleSystem(bool val)
    {
        if(_vfxParticleSystem == null)
        {
            return;
        }

        if(val)
        {
            _vfxParticleSystem.Play();
        }
        else
        {
            _vfxParticleSystem.Stop();
        }
    }
}
