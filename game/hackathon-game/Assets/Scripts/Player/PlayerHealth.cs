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
    public bool IsDead { get { return isDead; } set { isDead = value; } }
    public void Start()
    {
        SetMaxHealth();

        // Make sure the volume has a profile assigned
        if (damageVolume == null || damageVolume.profile == null)
        {
            Debug.LogError("Damage volume or profile is not assigned!");
            return;
        }

        // Get or add the vignette effect
        if (!damageVolume.profile.TryGet(out vignette))
        {
            // If vignette doesn't exist, add it to the profile
            vignette = damageVolume.profile.Add<Vignette>(false);
        }

        // Initialize vignette settings
        vignette.active = true;
        vignette.intensity.Override(0f);
        vignette.color.Override(Color.red);
    }

    public void SetMaxHealth()
    {
        _currentHealth = _maxHealth;

        // Set max health
        HUDManager._maxHealthUpdater?.Invoke(_currentHealth);
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
            Die(true);
        }

        // Refill health in tutorial
        if (TutorialManager.Instance != null) StartCoroutine(TutorialManager.Instance.RefillHealth());
    }

    private IEnumerator FlashRed()
    {
        if (vignette == null)
        {
            Debug.LogWarning("Vignette effect not found!");
            yield break;
        }

        float elapsedTime = 0f;
        float startIntensity = 0f;
        float targetIntensity = 0.5f;

        // Fade in
        while (elapsedTime < flashDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float newIntensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / (flashDuration / 2f));
            vignette.intensity.Override(newIntensity);
            yield return null;
        }

        // Ensure we reach the target intensity
        vignette.intensity.Override(targetIntensity);

        // Fade out
        elapsedTime = 0f;
        startIntensity = targetIntensity;
        targetIntensity = 0f;

        while (elapsedTime < flashDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float newIntensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / (flashDuration / 2f));
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

        GetComponentInChildren<GunManager>(true).gameObject.SetActive(false);
        GetComponentInChildren<Canvas>(true).gameObject.SetActive(false);

    }

    public void Die(bool shatter)
    {
        DisablePlayer();

        // Stop time count
        GameManager.Instance?.StopTimeCount();

        // Shatter Effect
        if (shatter)
        {
            GetComponent<Fracture>().Shatter();

            // Play sfx
            AudioManager.Instance.PlaySfx(AudioManager.Instance._playerDeathSfx);
            AudioManager.Instance.PlaySfx(AudioManager.Instance._playerDeath2Sfx);
        }


        // Trigger Death Panel
        if (TutorialManager.Instance == null) StartCoroutine(UIManager.Instance.DeathPanelSetup());

    }
}
