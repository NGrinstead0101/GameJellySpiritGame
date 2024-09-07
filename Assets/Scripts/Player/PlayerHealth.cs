/******************************************************************
 *    Author: Nick Grinstead
 *    Contributors: 
 *    Description: Tracks player health and updates the health bar.
 *******************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _deathFadeOutImage;
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _deathFadeOutTime = 1f;

    private float _currentHealth;

    /// <summary>
    /// Initial set-up
    /// </summary>
    private void Awake()
    {
        _deathFadeOutImage.gameObject.SetActive(false);
        _currentHealth = _maxHealth;

        // TODO: register to take damage action here
    }

    /// <summary>
    /// Unregisters from action
    /// </summary>
    private void OnDisable()
    {
        //TODO: unregister from damage action here
    }

    /// <summary>
    /// Code for testing in editor without enemies
    /// </summary>
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            TakeDamage(20f);
    }
#endif

    /// <summary>
    /// Called when the player takes damage. 
    /// Updates health bar and triggers death if needed.
    /// </summary>
    /// <param name="damage">Damage taken</param>
    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        // Update health bar
        float healthBarFill = _currentHealth / _maxHealth;
        healthBarFill = Mathf.Clamp(healthBarFill, 0, _maxHealth);
        _healthBar.fillAmount = healthBarFill;

        // Check for death
        if (_currentHealth <= 0)
        {
            Time.timeScale = 0;
            StartCoroutine(nameof(DeathFadeOutTimer));
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
