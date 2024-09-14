/******************************************************************
 *    Author: Nick Grinstead
 *    Contributors: 
 *    Description: Tracks player health and updates the health bar.
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    public static Action TakeDamageAction;
    public static Action DeathAction;

    [SerializeField] private Sprite _devilHeart;
    [SerializeField] private Sprite _angelHeart;
    [SerializeField] private Sprite _emptyHeart;

    [SerializeField] private GameObject _heartPrefab;
    [SerializeField] private Transform _healthBar;
    [SerializeField] private Image _deathFadeOutImage;
    public int MaxHealth = 5;
    [SerializeField] private float _deathFadeOutTime = 1f;

    //private int _totalHealth;
    public int CurrentHealth { get; private set; }
    private List<Image> _hearts = new List<Image>();

    private GameManager _gameManager;
    private PlayerController _playerController;

    private const float StartingHeartPos = -345f;
    private const float ChangeInHeartPos = 40f;

    /// <summary>
    /// Initial set-up
    /// </summary>
    private void Awake()
    {
        // Set-up health bar
        for (int i = 0; i < MaxHealth; ++i)
        {
            GameObject newHeart = Instantiate(_heartPrefab, _healthBar);
            newHeart.transform.localPosition = 
                new Vector2(StartingHeartPos + ChangeInHeartPos * i, newHeart.transform.localPosition.y);
            _hearts.Add(newHeart.GetComponent<Image>());
        }

        _deathFadeOutImage.gameObject.SetActive(false);
        CurrentHealth = MaxHealth;

        // TODO: register to take damage and heal action here

        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    
        EnemyStateMachine.DealtDamage += TakeDamage;
        // TODO: register heal action here
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        UpdateHeartAppearance(_gameManager.GetCurrentAbilityType());

        PlayerController.SwapForm += UpdateHeartAppearance;
    }

    private void UpdateHeartAppearance(AbilitySetType form)
    {
        for (int i = 0; i < _hearts.Count; ++i)
        {
            if (i >= CurrentHealth)
            {
                _hearts[i].GetComponent<Image>().sprite = _emptyHeart;
            }
            else
            {
                _hearts[i].GetComponent<Image>().sprite = form == AbilitySetType.Angel ? _angelHeart : _devilHeart;
            }
        }
    }

    /// <summary>
    /// Unregisters from action
    /// </summary>
    private void OnDisable()
    {
        EnemyStateMachine.DealtDamage -= TakeDamage;
    }

    /// <summary>
    /// Code for testing in editor without enemies
    /// </summary>
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            TakeDamage();
        if (Input.GetKeyDown(KeyCode.P))
            HealPlayer(1);
    }
#endif

    /// <summary>
    /// Called when the player takes damage. 
    /// Updates health bar and triggers death if needed.
    /// </summary>
    /// <param name="damage">Damage taken</param>
    public void TakeDamage()
    {
        // Update health bar
        if (CurrentHealth > 0 && CurrentHealth <= _hearts.Count)
        {
            _hearts[CurrentHealth - 1].GetComponent<Image>().sprite = _emptyHeart;

            CurrentHealth--;

            TakeDamageAction?.Invoke();
        }

        // Check for death
        if (CurrentHealth <= 0)
        {
            DeathAction?.Invoke();
            //Time.timeScale = 0;
            StartCoroutine(nameof(DeathFadeOutTimer));
        }
    }

    /// <summary>
    /// Called when angel heal ability is activated to restore health
    /// </summary>
    /// <param name="healthGained">Amount of hearts restored</param>
    public void HealPlayer(int healthGained)
    {
        CurrentHealth += healthGained;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        AbilitySetType form = _gameManager.GetCurrentAbilityType();
        UpdateHeartAppearance(form);

        //// Update health bar
        //for (int i = 0; i < CurrentHealth && i < _hearts.Count; ++i)
        //{
        //    _hearts[i].GetComponent<SpriteRenderer>().sprite = form == AbilitySetType.Angel ? : _angelHeart 
        //}
    }

    /// <summary>
    /// On death, fades screen to black before reloading scene
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeathFadeOutTimer()
    {
        float timeWaited = 0f;
        float imageAlpha = 0f;

        _deathFadeOutImage.gameObject.SetActive(true);

        while (timeWaited < _deathFadeOutTime)
        {
            yield return new WaitForSecondsRealtime(0.05f);

            timeWaited += 0.05f;
            
            imageAlpha = Mathf.Lerp(0f, 1f, timeWaited / _deathFadeOutTime);
            _deathFadeOutImage.color = new Color(0, 0, 0, imageAlpha);
        }

        Time.timeScale = 1f;

        ReloadScene();
    }

    private void ReloadScene()
    {
        StopAllCoroutines();

        Scene tempScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(tempScene.name);
    }
}
