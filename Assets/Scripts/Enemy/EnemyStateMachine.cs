using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyStateMachine : MonoBehaviour
{
    private Vector2 targetDestination;
    private bool isFacingLeft;
    private EnemyState currentState;
    private GameObject player;
    private bool isAggressive;
    private bool isNextToPlayer;
    [SerializeField] private Transform frontDetectionPoint;
    [SerializeField] private int frontDetectionPointDistance;
    [SerializeField] private int frontMeleeDistance;
    [SerializeField] private float EnemySpeed;
    public static UnityAction HitPlayer;

    public void Start()
    {
        currentState = new Idle();
        StartCoroutine(MoveTowardsTarget());
        targetDestination = GetPlayerLocation();
    }
    public void Update()
    {
        while (currentState != null)
        {

        }
    }

    /// <summary>
    /// When running, moves the enemy towards a set location on the x axis.
    /// </summary>
    /// <returns></returns>
    public IEnumerator MoveTowardsTarget()
    {
        while(true)
        {
            Vector2 loc = targetDestination;
            float direction = Mathf.Sign(loc.x - transform.position.x);

            Vector2 currentPosition = transform.position;
            currentPosition.x += direction * GetSpeed() * Time.deltaTime;

            transform.position = new Vector2(currentPosition.x, transform.position.y);
            yield return new WaitForFixedUpdate();
        }
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
    public void ChangeState(EnemyState newState)
    {
        if (currentState != null)
            currentState.Exit(this);

        currentState = newState;
        currentState.Enter(this);
    }
    public bool IsFacingLeft()
    {
        if (!isFacingLeft) //if youre not facing left
        {
            return false; //then youre not facing left
        }
        return true;
    }
    public void ChangeFacingDirection()
    {
        if (!isFacingLeft)
        {
            isFacingLeft = true;
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            isFacingLeft = false;
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }
    public GameObject GetPlayer()
    {
        if (player != null)
        {
            return player;
        }
        return GameObject.FindGameObjectWithTag("Player");
    }
    public Vector2 GetPlayerLocation()
    {
        if (player != null)
        {
            return player.transform.position;
        }
        return GetPlayer().transform.position;
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
        return EnemySpeed;
    }
    public bool IsFacingPlayer()
    {
        if((transform.position.x < GetPlayerLocation().x) && !isFacingLeft)
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
    public bool GetNextToPlayer()
    {
        return isNextToPlayer;
    }
    public void ChangeIsNextToPlayer()
    {
        if(!isNextToPlayer)
        {
            isNextToPlayer = true;
        }
        else
        {
            isNextToPlayer = false;
        }
    }
    public void SetDestination(Vector2 destination)
    {
        targetDestination = destination;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HitPlayer?.Invoke();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            ChangeIsNextToPlayer();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ChangeIsNextToPlayer();
        }
    }
}
