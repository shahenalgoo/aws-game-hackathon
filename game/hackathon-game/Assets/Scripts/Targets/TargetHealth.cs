using UnityEngine;

public class TargetHealth : MonoBehaviour
{

    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth;
    private HealthBar healthBar;

    // Reference to the health bar prefab
    [SerializeField] private GameObject healthBarPrefab;
    // Offset for health bar position
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0, 2f, 0);

    public void Start()
    {
        _currentHealth = _maxHealth;

        // Spawn the health bar
        CreateHealthBar();
    }

    private void CreateHealthBar()
    {
        // Instantiate the health bar prefab as a child of this object
        GameObject healthBarObject = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity, transform);

        // Get the HealthBar component
        healthBar = healthBarObject.GetComponent<HealthBar>();

        // Set the initial max health
        healthBar.SetMaxHealth(_maxHealth);
    }
    public void TakeDamage(int amount)
    {

        _currentHealth -= amount;

        // Update the health bar
        healthBar.UpdateHealthBar(_currentHealth);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);

        // Spawn loot
        Instantiate(GameManager.Instance.TargetLoot, transform.position, Quaternion.identity);
    }
}
