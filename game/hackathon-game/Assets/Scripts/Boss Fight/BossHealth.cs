using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth;

    [SerializeField] private BossHUDManager _bossHUD;
    [SerializeField] private BossController _bossController;
    private bool _isDead;
    public bool IsDead => _isDead;


    public void Start()
    {
        SetMaxHealth();
    }

    public void SetMaxHealth()
    {
        _currentHealth = _maxHealth;
        _bossHUD.SetMaxHealthBar(_maxHealth);
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        // Update the health bar
        _bossHUD.SetTargetHealth(_currentHealth);


        // Activate 2nd phase at 2/3 health
        if (!_bossController.PhaseTwoActivated && _currentHealth <= Mathf.CeilToInt(_maxHealth * 0.66f))
        {
            _bossController.ActivatePhaseTwo();
        }

        // Activate 3rd phase at 1/3 health

        else if (!_bossController.PhaseThreeActivated && _currentHealth <= Mathf.CeilToInt(_maxHealth * 0.33f))
        {
            _bossController.ActivatePhaseThree();
        }

        // Check death
        if (_currentHealth <= 0) Die();
    }

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;
        _bossHUD.gameObject.SetActive(false);
        _bossController.SawBlade.gameObject.SetActive(false);

        // Find extraction platform and activate it
        FindObjectOfType<ExtractionPlatformFadeIn>().StartFadeIn();

        // Fade out music
        StartCoroutine(AudioManager.Instance.FadeOutBossMusic(2f));

        // Explode boss
        GetComponent<BossController>().Explode();

        // Disable collision
        GetComponent<CapsuleCollider>().enabled = false;

    }
}
