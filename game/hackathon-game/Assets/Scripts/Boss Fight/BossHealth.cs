using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth;

    [SerializeField] private BossHUDManager _bossHUD;
    [SerializeField] private BossController _bossController;


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

        if (_currentHealth <= 0)
        {
            Die();
            return;
        }

        if (!_bossController.PhaseTwoActivated && _currentHealth <= Mathf.CeilToInt(_maxHealth * 0.66f))
        {
            _bossController.ActivatePhaseTwo();
        }
        else if (!_bossController.PhaseThreeActivated && _currentHealth <= Mathf.CeilToInt(_maxHealth * 0.33f))
        {
            _bossController.ActivatePhaseThree();
        }

    }

    public void Die()
    {
        _bossHUD.gameObject.SetActive(false);
        _bossController.SawBlade.gameObject.SetActive(false);

        // Find extraction platform and activate it
        GameObject.FindObjectOfType<ExtractionPlatformFadeIn>().StartFadeIn();


        transform.parent.gameObject.SetActive(false);
    }
}