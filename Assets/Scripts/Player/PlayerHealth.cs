/******************************************************************
 *    Author: Nick Grinstead
 *    Contributors: 
 *    Description: Tracks player health and updates the health bar.
 *******************************************************************/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private GameObject _heartPrefab;
    [SerializeField] private Transform _healthBar;
    [SerializeField] private Image _deathFadeOutImage;
    [SerializeField] private int _totalHealth = 5;
    [SerializeField] private float _deathFadeOutTime = 1f;

    //private int _totalHealth;
    private int _currentHealth;
    private List<Image> _hearts = new List<Image>();

    private const float StartingHeartPos = -345f;
    private const float ChangeInHeartPos = 40f;

    /// <summary>
    /// Initial set-up
    /// </summary>
    private void Awake()
    {
        // Set-up health bar
        for (int i = 0; i < _totalHealth; ++i)
        {
            GameObject newHeart = Instantiate(_heartPrefab, _healthBar);
            newHeart.transform.localPosition = 
                new Vector2(StartingHeartPos + ChangeInHeartPos * i, newHeart.transform.localPosition.y);
            _hearts.Add(newHeart.GetComponent<Image>());
        }

        _deathFadeOutImage.gameObject.SetActive(false);
        _currentHealth = _totalHealth;

        EnemyState.DealtDamage += TakeDamage;
        // TODO: register heal action here
    }

    /// <summary>
    /// Unregisters from action
    /// </summary>
    private void OnDisable()
    {
        //TODO: unregister from damage and heal action here
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
        if (_currentHealth > 0 && _currentHealth <= _hearts.Count)
        {
            _hearts[_currentHealth - 1].enabled = false;

            _currentHealth--; ;
        }

        // Check for death
        if (_currentHealth <= 0)
        {
            Time.timeScale = 0;
            StartCoroutine(nameof(DeathFadeOutTimer));
        }
    }

    /// <summary>
    /// Called when angel heal ability is activated to restore health
    /// </summary>
    /// <param name="healthGained">Amount of hearts restored</param>
    public void HealPlayer(int healthGained)
    {
        _currentHealth += healthGained;
        if (_currentHealth > _totalHealth)
        {
            _currentHealth = _totalHealth;
        }

        // Update health bar
        for (int i = 0; i < _currentHealth && i < _hearts.Count; ++i)
        {
            _hearts[i].enabled = true;
        }
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

        // TODO: replace these two lines once a scene manager is built
        Scene tempScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(tempScene.name);
    }
}
