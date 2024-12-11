using UnityEngine;

public class LaserBotDamager : MonoBehaviour
{
    [SerializeField] private int _damage = 15;
    [SerializeField] private float _knockbackForce = 1f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !other.GetComponent<PlayerStateMachine>().IsDashing)
        {
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
        }
    }
}
