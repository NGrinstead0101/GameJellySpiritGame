using System.Collections;
using UnityEngine;
using static System.TimeZoneInfo;

public class Ability_LungeAttack : Ability
{
    [Tooltip("Collider offset at the max attack range")]
    [SerializeField] float _attackOffset;

    [Tooltip("Duration the hitbox will move forward, smaller value is a faster attack")]
    [SerializeField] float _forwardAttackDuration;

    [Tooltip("Time after the forward attack before the hitbox starts to return to the base position")]
    [SerializeField] float _returnDelay;

    [Tooltip("Duration the hitbox will move towards the base position, smaller value is a faster attack")]
    [SerializeField] float _returnDuration;

    private Collider2D _hitBoxCollider = null;
    private Vector2 _baseHitboxOffset = Vector2.zero;
    private bool _canAttack = true;

    private void Start()
    {
        _hitBoxCollider = GetComponent<Collider2D>();
        _baseHitboxOffset = _hitBoxCollider.offset;
    }
    
    /// <summary>
    /// Casts the lunge attack ability
    /// </summary>
    public override void CastAbility()
    {
        if (_canAttack)
        {
            _canAttack = false;
            base.CastAbility();

            //Debug.Log(_abilityInformation.name + " Child Casted");
            StartCoroutine(MoveHitBox());
        }
    }

    /// <summary>
    /// Moves hitbox towards the attack offset
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveHitBox()
    {
        float pathTime = 0;
        Vector2 endingOffset = new Vector2(_attackOffset, _hitBoxCollider.offset.y);

        while (pathTime < _forwardAttackDuration)
        {
            float pathPercent = (pathTime / _forwardAttackDuration);

            Vector2 newXOffset = Vector2.Lerp(_baseHitboxOffset, endingOffset, pathPercent);
            _hitBoxCollider.offset = newXOffset;

            pathTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(_returnDelay);
        StartCoroutine(ReturnHitBox());
    }

    /// <summary>
    /// Returns the hitbox to the base position
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReturnHitBox()
    {
        float pathTime = 0;
        
        Vector2 startingOffset = new Vector2(_attackOffset, _hitBoxCollider.offset.y);

        while (pathTime < _returnDuration)
        {
            float pathPercent = (pathTime / _forwardAttackDuration);

            Vector2 newXOffset = Vector2.Lerp(startingOffset, _baseHitboxOffset, pathPercent);
            _hitBoxCollider.offset = newXOffset;

            pathTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(_cooldown);
        _canAttack = true;
    }
}
