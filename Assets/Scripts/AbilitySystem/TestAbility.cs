using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAbility : Ability
{
    public override void CastAbility()
    {
        base.CastAbility();

        Debug.Log(_abilityInformation.name + " Child Casted");
    }
}
