using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth;

    public void Start()
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


        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {

        // Die visuals & audio feedback

        // Remove this later
        Destroy(gameObject);

        // Death screen
    }
}
