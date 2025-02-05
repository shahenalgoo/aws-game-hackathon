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
            // Play sfx
            AudioManager.Instance.PlaySfx(AudioManager.Instance._playerHurtMeleeSfx);

            int damageRoundUp = Mathf.CeilToInt(_damage);
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damageRoundUp);
            playerHealth.DamageVfx.Play();
        }
    }
}
