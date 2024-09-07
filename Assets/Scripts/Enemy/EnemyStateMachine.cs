using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyStateMachine : MonoBehaviour
{
    public static Action EnemyDied;

    private float time;
    private Vector2 targetDestination;
    private bool isFacingLeft;
    private EnemyState currentState;
    private GameObject player;
    private bool isAggressive;
    private bool isNextToPlayer;
    [SerializeField] private Transform frontDetectionPoint;
    [SerializeField] private int frontDetectionPointDistance;
    [SerializeField] private int frontMeleeDistance;
    [SerializeField] private int chaseDistance;
    [SerializeField] private float EnemySpeed;
    [SerializeField] private float TickSpeed;
    [SerializeField] private float MaxHealth;
    private float currentHealth;
    private float currentSpeed;
    public static UnityAction HitPlayer;
    private bool isNearWall = false;

    public void Start()
    {
        if (currentState == null)
        {
            currentState = new Idle();
        }
        currentState.Enter(this);
        StartCoroutine(MoveTowardsTarget());
    }
    public void FixedUpdate()
    {
        Vector2 loc = targetDestination;
        float direction = Mathf.Sign(loc.x - transform.position.x);

        Vector2 currentPosition = transform.position;
        currentPosition.x += direction * GetSpeed() * Time.deltaTime;

        transform.position = new Vector2(currentPosition.x, transform.position.y);
    }

    /// <summary>
    /// When running, moves the enemy towards a set location on the x axis.
    /// </summary>
    /// <returns></returns>
    public IEnumerator MoveTowardsTarget()
    {
        while (currentState != null)
        {
            if (currentState.NeedsDestinationUpdate(this))
            {
                Debug.Log("Updating destination");
                currentState.UpdateDestination(this);
            }

           
            yield return new WaitForSeconds(TickSpeed);
        }
    }
    private IEnumerator Timer()
    {
        time = 0; //reset the timer
        while(true)
        {
            time += Time.deltaTime;
            yield return null;
        }
    }

    public float GetTime()
    {
        return time;
    }
    public bool IsInRange()
    {
        if (Vector2.Distance(transform.position, GetPlayerLocation()) < GetChaseDistance())
        {
            Debug.Log("is in range");
            return true;
        }
        Debug.Log("Not in range");
        return false;
    }
    public int GetChaseDistance()
    {
        return chaseDistance;
    }
    public Vector2 GetPlayerLocation()
    {
        if (player != null)
        {
            return player.transform.position;
        }
        return GetPlayer().transform.position;
    }
    public Vector2 GetDestination()
    {
        return targetDestination;
    }
    public EnemyState GetState()
    {
        if (currentState != null)
        {
            return currentState;
        }
        Debug.LogError("Enemy does not have a state");
        return null;
    }
    public bool GetIsFacingLeft()
    {
        if (!isFacingLeft) //if youre not facing left
        {
            return false; //then youre not facing left
        }
        return true;
    }
    public GameObject GetPlayer()
    {
        if (player != null)
        {
            return player;
        }
        return GameObject.FindGameObjectWithTag("Player");
    }
    public bool GetAggressionState()
    {
        return isAggressive;
    }
    public Transform GetDetectionPoint()
    {
        return frontDetectionPoint;
    }
    public float GetSpeed()
    {
        return currentSpeed;
    }
    public float GetNormalSpeed()
    {
        return EnemySpeed;
    }
    public float GetMaxHealth()
    {
        return MaxHealth;
    }
    public bool GetNextToPlayer()
    {
        Debug.Log("Next to player: " + isNextToPlayer);
        return isNextToPlayer;
    }
    public bool IsFacingPlayer()
    {
        if ((transform.position.x < GetPlayerLocation().x) && !isFacingLeft)
        {
            return true;
        }
        return false;
    }
    public bool ChangeAggression()
    {
        if (!isAggressive)
        {
            isAggressive = true;
        }
        else
        {
            isAggressive = false;
        }
        return isAggressive;
    }
    public bool IsNextToEdge()
    {
        RaycastHit2D hit = (Physics2D.Raycast(GetDetectionPoint().position, -Vector2.up, frontDetectionPointDistance));
        {
            if (hit.collider != null)
            {
                return false;
            }
            return true;
        }
    }
    public bool IsNextToWall()
    {
        Debug.Log("next to wall called");
        bool returnVal = isNearWall;

        if (isNearWall == true)
            isNearWall = false;

        return returnVal;
    }

    public void ChangeIsNextToPlayer(bool isPlayerClose)
    {
        isNextToPlayer = isPlayerClose;
    }

    public void SetDestination(Vector2 destination)
    {
        targetDestination = destination;
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState != null)
            currentState.Exit(this);

        currentState = newState;
        currentState.Enter(this);
    }

    public void ChangeFacingDirection()
    {
        if (!isFacingLeft)
        {
            isFacingLeft = true;
            gameObject.transform.localScale = new Vector3(-1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        }
        else
        {
            isFacingLeft = false;
            gameObject.transform.localScale = new Vector3(1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        }
    }

    public void ChangeSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
    }

    public void StartTimer()
    {
        StartCoroutine(Timer());
    }

    public void StopTimer()
    {
        StopCoroutine(Timer());
    }

    public void AddHealth(float healthToRemove)
    {
        currentHealth = Mathf.Floor(currentHealth + healthToRemove);
    }

    public void RemoveHealth(float healthToRemove)
    {
        currentHealth -= healthToRemove;
        if (currentHealth <= 0)
        {
            EnemyDied?.Invoke();
            ChangeState(new Died());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HitPlayer?.Invoke();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ChangeIsNextToPlayer(true);
        }

        if (other.CompareTag("WALL"))
        {
            Debug.Log("TriggerEntered");
            isNearWall = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ChangeIsNextToPlayer(false);
        }
    }
}
