using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Two ability sets: Angel and Devil
/// </summary>
public enum AbilitySetType
{
    Angel,
    Devil
}

/// <summary>
/// Contains the ability set type and dictionary of abilities
/// </summary>
public struct AbilitySet
{
    public AbilitySetType SetType { get; private set; }
    public Dictionary<int, Ability> Abilities { get; private set; }

    public AbilitySet(AbilitySetType setType)
    {
        SetType = setType;
        Abilities = new Dictionary<int, Ability>();
    }
}

public class AbilitySystem : MonoBehaviour
{
    public static AbilitySystem Instance;

    public Action<int> OnCastAbility;

    [SerializeField] private GameObject[] _angelAbilities;
    [SerializeField] private GameObject[] _devilAbilities;

    /// <summary>
    /// Initializes the angel and devil ability structs
    /// </summary>
    AbilitySet _angelAbilitySet = new AbilitySet(AbilitySetType.Angel);
    AbilitySet _devilAbilitySet = new AbilitySet(AbilitySetType.Devil);

    // Current active ability set
    private static AbilitySet _activeAbilitySet;

    private static InputAction _castAbility = null;

    #region Assign Actions
    private void OnEnable()
    {
        PlayerController.SwapForm += SwitchAbilitySet;
        OnCastAbility += CastAbility;
    }
    private void OnDisable()
    {
        PlayerController.SwapForm -= SwitchAbilitySet;
        OnCastAbility -= CastAbility;

        _castAbility.performed -= ctx => CastAbility(0);
        _castAbility.canceled -= ctx => CancelAbility(0);

    }
    #endregion Assign Actions

    #region Awake
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
    #endregion Awake

    private void Start()
    {
        // Initializes Angel Abilities Dictionary
        for(int i = 0; i < _angelAbilities.Length; i++)
        {
            if (_angelAbilities[i].TryGetComponent<Ability>(out Ability ability))
            {
                _angelAbilitySet.Abilities.Add(i, ability);
            }
        }

        // Initializes Devil Abilities Dictionary
        for (int i = 0; i < _devilAbilities.Length; i++)
        {
            if (_devilAbilities[i].TryGetComponent<Ability>(out Ability ability))
            {
                _devilAbilitySet.Abilities.Add(i, ability);
            }
        }
    }

    /// <summary>
    /// Binds the cast ability action to GameplayInputs. Callled in PlayerController awake
    /// </summary>
    public static void BindUseAbility()
    {
        _castAbility = PlayerController.GameplayInputs.FindAction("UseAbility");
        _castAbility.performed += ctx => CastAbility(0);
    }

    private static void _castAbility_started(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Switches to a new ability set
    /// </summary>
    /// <param name="newAbilitySet"></param>
    private void SwitchAbilitySet(AbilitySetType newAbilitySet)
    {
        switch (newAbilitySet)
        {
            case AbilitySetType.Angel:
                {
                    _activeAbilitySet = _angelAbilitySet;

                    _castAbility.canceled += ctx => CancelAbility(0);

                    Debug.Log("SwitchedToAngel");       
                    break;
                }
            case AbilitySetType.Devil:
                {
                    _activeAbilitySet = _devilAbilitySet;
                    Debug.Log("SwitchedToDevil");
                    break;
                }
        }
    }

    /// <summary>
    /// Casts the numAbility from the current active set
    /// </summary>
    /// <param name="numAbility">num ability being cast</param>
    private static void CastAbility(int numAbility)
    {
        _activeAbilitySet.Abilities[numAbility].CastAbility();
    }

    private static void CancelAbility(int numAbility)
    {
        _activeAbilitySet.Abilities[numAbility].CancelAbility();
    }
}
