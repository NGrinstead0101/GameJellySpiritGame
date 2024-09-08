/******************************************************************
 *    Author: Nick Grinstead
 *    Contributors: 
 *    Description: Behaviors for end of level doors.
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    [SerializeField] private bool _isLocked = false;
    [SerializeField] private int _enemyCount;

    private bool _hasTriggeredDoor = false;

    /// <summary>
    /// Registering to action
    /// </summary>
    private void Awake()
    {
        _isLocked = _enemyCount <= 0 ? false : true;

        // TODO: register to enemy death action here
    }

#if UNITY_EDITOR
    // Testing code for manually killing enemies
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            CheckForDoorUnlock();
    }
#endif

    /// <summary>
    /// Uregistering from action
    /// </summary>
    private void OnDisable()
    {
        // TODO: unregister from enemy death action here
    }

    /// <summary>
    /// When enemy dies, checks if door should unlock
    /// </summary>
    private void CheckForDoorUnlock()
    {
        _enemyCount--;

        _isLocked = _enemyCount <= 0 ? false : true;
    }

    /// <summary>
    /// Checks for player walking through door
    /// </summary>
    /// <param name="collision">Collider involved in collision</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !_isLocked && !_hasTriggeredDoor)
        {
            _hasTriggeredDoor = true;

            // TODO: trigger end of level here
            Debug.Log("Reached End of Level");
        }
    }
}
