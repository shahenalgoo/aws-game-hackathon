using UnityEngine;

public class TargetBulletController : Bullet
{
    [SerializeField] private float _knockbackForce = 10f;

    [SerializeField] private GameObject _impact;

    // Or add this function for physics-based collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !other.gameObject.GetComponent<PlayerStateMachine>().IsDashing)
        {
            Vector3 collisionPoint = other.ClosestPoint(transform.position);

            Instantiate(_impact, collisionPoint, Quaternion.identity);

            int damageRoundUp = Mathf.CeilToInt(_currentDamage);
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damageRoundUp);
            playerHealth.DamageVfx.Play();


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
