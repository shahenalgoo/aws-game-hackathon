using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeController : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Vector3 collisionPoint = other.ClosestPoint(transform.position);

            int damageRoundUp = Mathf.CeilToInt(_damage);
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damageRoundUp);
            playerHealth.DamageVfx.Play();
        }
    }
}
