using UnityEngine;

public class TargetController : MonoBehaviour
{

    [SerializeField] private int health = 100;
    // Start is called before the first frame update
    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
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
