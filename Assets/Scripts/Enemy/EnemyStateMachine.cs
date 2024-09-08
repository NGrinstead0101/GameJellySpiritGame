using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyStateMachine : MonoBehaviour
{
    public static Action EnemyDied;

    [SerializeField] private GameObject torchPrefab;
    [SerializeField] private bool shouldSpawnTorch = false;
    [SerializeField] private GameObject _hitVfxObject;
    [SerializeField] private GameObject _deathVfxObject;

    private SfxManager _sfxManager;
    private SpriteRenderer _spriteRenderer;
    private ParticleSystem _hitVfxSystem;
    private ParticleSystem _deathVfxSystem;
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
    private bool isNearLedge = false;
    private Animator enemyAnimator;

    public void Start()
    {
        _sfxManager = SfxManager.Instance;

        _deathVfxSystem = _deathVfxObject.GetComponent<ParticleSystem>();
        _hitVfxSystem = _hitVfxObject.GetComponent<ParticleSystem>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        SetDeathVfxSystem(false);
        SetHitVfxSystem(false);

        enemyAnimator = GetComponent<Animator>();
        currentHealth = MaxHealth;

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

    private IEnumerator DisableHitVfxDelay()
    {
        yield return new WaitForSeconds(0.1f);

        SetHitVfxSystem(false);
    }

    private void SetHitVfxSystem(bool isActive)
    {
        if (_hitVfxSystem == null)
            return;

        if (isActive)
        {
            _spriteRenderer.color = Color.red;
            _hitVfxSystem.Play();
        }
        else
        {
            _spriteRenderer.color = Color.white;
            _hitVfxSystem.Stop();
        }
    }

    private void SetDeathVfxSystem(bool isActive)
    {
        if (_deathVfxSystem == null)
            return;

        if (isActive)
        {
            _spriteRenderer.color = Color.red;
            _deathVfxSystem.Play();
        }
        else
        {
            _spriteRenderer.color = Color.white;
            _deathVfxSystem.Stop();
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
            return true;
        }
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
        bool returnVal = isNearLedge;

        if(isNearLedge)
            isNearLedge = false;

        return returnVal;
    }
    public bool IsNextToWall()
    {
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
            gameObject.transform.localScale = new Vector3(-gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        }
        else
        {
            isFacingLeft = false;
            gameObject.transform.localScale = new Vector3(-gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
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

        if (currentHealth > 0)
        {
            PlaySfx("EnemyHit");
            SetHitVfxSystem(true);
            StartCoroutine(DisableHitVfxDelay());
        }
        else if (currentHealth <= 0)
        {
            SetDeathVfxSystem(true);
            PlaySfx("EnemyDeath");

            Invoke(nameof(DeathDelay), 0.15f);
        }
    }

    private void DeathDelay()
    {
        EnemyDied?.Invoke();

        if (shouldSpawnTorch)
            Instantiate(torchPrefab, transform.position, Quaternion.identity);

        ChangeState(new Died());
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
            isNearWall = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ChangeIsNextToPlayer(false);
        }

        if (other.CompareTag("Ground"))
        {
            isNearLedge = true;
        }
    }

    public void PlaySfx(string sfxName)
    {
        _sfxManager.PlaySFX(sfxName);
    }

    public void SpotPlayer()
    {
        enemyAnimator.SetBool("hasSpottedPlayer", true);
    }    

    public void ToggleIsAttacking(bool isAttacking)
    {
        enemyAnimator.SetBool("isAttacking", isAttacking);
    }

    private void OnDisable()
    {
        SetDeathVfxSystem(false);
    }
}
