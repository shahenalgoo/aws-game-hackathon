using UnityEngine;

public class TurbineBladeController : MonoBehaviour
{
    [SerializeField] private int _damage = 15;
    [SerializeField] private float _knockbackForce = 1f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !other.GetComponent<PlayerStateMachine>().IsDashing)
        {

            // Play SFX
            AudioManager.Instance.PlaySfx(AudioManager.Instance._playerHurtSharpSfx);

            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(_damage);
            playerHealth.DamageVfx.Play();

            // Add knockback using CharacterController
            CharacterController playerCC = other.gameObject.GetComponent<CharacterController>();
            if (playerCC != null)
            {
                Vector3 collisionPoint = other.ClosestPoint(transform.position);
                // Calculate the tangential direction at the point of impact
                Vector3 radiusVector = collisionPoint - transform.position;
                Vector3 rotationAxis = transform.up; // Assuming the blade rotates around its Y-axis
                Vector3 knockbackDirection = Vector3.Cross(rotationAxis, radiusVector).normalized;


                // Apply the knockback
                PlayerStateMachine psm = other.gameObject.GetComponent<PlayerStateMachine>();
                if (psm != null)
                {
                    psm.ApplyKnockback(knockbackDirection * _knockbackForce);
                }
            }
        }
    }
}
