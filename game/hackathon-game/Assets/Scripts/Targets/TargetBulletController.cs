using UnityEngine;

public class TargetBulletController : Bullet
{
    [SerializeField] private float _knockbackForce = 10f;

    // Or add this function for physics-based collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !other.gameObject.GetComponent<PlayerStateMachine>().IsDashing)
        {
            int damageRoundUp = Mathf.CeilToInt(_currentDamage);
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damageRoundUp);

            // Add knockback using CharacterController
            CharacterController playerCC = other.gameObject.GetComponent<CharacterController>();
            if (playerCC != null)
            {
                // Use the bullet's forward direction for the knockback
                Vector3 knockbackDirection = transform.forward;

                // Apply the knockback through your player movement script
                PlayerStateMachine psm = other.gameObject.GetComponent<PlayerStateMachine>();
                if (psm != null)
                {
                    psm.ApplyKnockback(knockbackDirection * _knockbackForce);
                }
            }

            CancelInvoke("DisableBullet");
            DisableBullet();
        }



    }
}
