using System.Collections;
using UnityEngine;

public class LaserBotDamager : MonoBehaviour
{
    private float _cooldown = 0.1f;
    private bool _canDamage = true;
    [SerializeField] private int _damage = 15;
    [SerializeField] private float _knockbackForce = 1f;

    void OnTriggerEnter(Collider other)
    {
        if (_canDamage && other.gameObject.CompareTag("Player") && !other.GetComponent<PlayerStateMachine>().IsDashing)
        {
            _canDamage = false;

            // Play sfx
            AudioManager.Instance.PlaySfx(AudioManager.Instance._playerHurtLaserSfx);

            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(_damage);
            playerHealth.DamageVfx.Play();

            // Add knockback using CharacterController
            CharacterController playerCC = other.gameObject.GetComponent<CharacterController>();
            if (playerCC != null)
            {
                // Use the player's negative forward for direction
                Vector3 knockbackDirection = -playerCC.transform.forward;

                // Apply the knockback through your player movement script
                PlayerStateMachine psm = other.gameObject.GetComponent<PlayerStateMachine>();
                if (psm != null)
                {
                    psm.ApplyKnockback(knockbackDirection * _knockbackForce);
                }
            }

            // Trigger cooldown
            StartCoroutine(CooldownLaser(_cooldown));
        }
    }

    private IEnumerator CooldownLaser(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _canDamage = true;
    }
}
