using System;
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

    [SerializeField] private ParticleSystem damageVfx;
    public ParticleSystem DamageVfx { get { return damageVfx; } }

    [SerializeField] public GameObject fallingTrailsObj;

    private bool isDead;
    public bool IsDead { get { return isDead; } }
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

    public void DisablePlayer()
    {
        isDead = true;

        // Disable controls, collision etc
        GetComponent<CharacterController>().detectCollisions = false;

        PlayerStateMachine psm = GetComponent<PlayerStateMachine>();
        psm.CharacterAnimator.SetBool("isReloading", false);
        psm.ToggleRigAndWeapon(false);
        psm._dashLightning.gameObject.SetActive(false);
        psm.CanDash = false;
        psm.enabled = false;

        GetComponentInChildren<GunManager>().gameObject.SetActive(false);
        GetComponentInChildren<Canvas>().gameObject.SetActive(false);

    }


    private void Die()
    {
        DisablePlayer();

        // Shatter Effect
        GetComponent<PlayerFracture>().Shatter();

        // Stop time count
        GameManager.Instance.CanCountTime = false;
    }
}
