/******************************************************************
 *    Author: Nick Grinstead
 *    Contributors: Marissa Moser
 *    Description: Behaviors for end of level doors.
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    [SerializeField] private bool _isLocked = false;
    [SerializeField] private int _enemyCount;
    [SerializeField] private Sprite _lockedDoorSprite;
    [SerializeField] private Sprite _unlockedDoorSprite;

    private bool _hasTriggeredDoor = false;
    private SpriteRenderer _spriteRenderer;

    /// <summary>
    /// Registering to action
    /// </summary>
    private void Awake()
    {
        _isLocked = _enemyCount <= 0 ? false : true;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _isLocked ? _lockedDoorSprite : _unlockedDoorSprite;

        EnemyStateMachine.EnemyDied += CheckForDoorUnlock;
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
        EnemyStateMachine.EnemyDied -= CheckForDoorUnlock;
    }

    /// <summary>
    /// When enemy dies, checks if door should unlock
    /// </summary>
    private void CheckForDoorUnlock()
    {
        _enemyCount--;

        _isLocked = _enemyCount <= 0 ? false : true;

        if (!_isLocked)
        {
            SfxManager.Instance.PlaySFX("DoorOpening");
            _spriteRenderer.sprite = _unlockedDoorSprite;
        }
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

            StartCoroutine(collision.GetComponent<PlayerController>().EnterDoorTransition());
        }
    }
}
