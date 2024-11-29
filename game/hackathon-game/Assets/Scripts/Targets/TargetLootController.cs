using UnityEngine;

public class TargetLootController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.IncrementLoot();
            Destroy(gameObject);
        }
    }
}
