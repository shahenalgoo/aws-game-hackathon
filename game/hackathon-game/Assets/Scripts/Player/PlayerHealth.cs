using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // For URP

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth;

    [Header("Damage Effect")]
    [SerializeField] private Volume damageVolume;
    [SerializeField] private float flashDuration = 0.2f;
    private Vignette vignette;

    private PlayerFracture fracture;
    public void Start()
    {
        _currentHealth = _maxHealth;

        // Set max health
        HUDManager._maxHealthUpdater?.Invoke(_currentHealth);

        // Get the vignette effect from the volume
        if (damageVolume != null && damageVolume.profile.TryGet(out vignette))
        {
            // Initialize vignette settings
            vignette.intensity.Override(0f);
            vignette.color.Override(Color.red);
        }
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        // Update the health bar
        HUDManager._targetHealthUpdater?.Invoke(_currentHealth);

        // Trigger damage effect
        StartCoroutine(FlashRed());

        // Play hurt animation
        PlayerStateMachine psm = GetComponent<PlayerStateMachine>();
        if (!psm.IsStunned) psm.IsStunned = true;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        if (vignette == null) yield break;

        float elapsedTime = 0f;
        float startIntensity = 0f;
        float targetIntensity = 0.5f;

        // Fade in
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float newIntensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / (flashDuration / 2));
            vignette.intensity.Override(newIntensity);
            yield return null;
        }

        // Fade out
        elapsedTime = 0f;
        startIntensity = targetIntensity;
        targetIntensity = 0f;

        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float newIntensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / (flashDuration / 2));
            vignette.intensity.Override(newIntensity);
            yield return null;
        }

        vignette.intensity.Override(0f);
    }


    private void Die()
    {
        fracture = GetComponent<PlayerFracture>();

        // Disable any animations or other components first
        GetComponent<Animator>().enabled = false;

        GetComponent<CharacterController>().enabled = false;

        GetComponent<PlayerStateMachine>().enabled = false;

        GetComponentInChildren<GunManager>().gameObject.SetActive(false);

        // Shatter Effect
        fracture.Shatter();

        // Hide original robot
        // gameObject.SetActive(false);

        // Die visuals & audio feedback

        // Death screen

    }
}
