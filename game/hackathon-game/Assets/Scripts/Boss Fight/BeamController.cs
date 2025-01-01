
using System.Collections;
using UnityEngine;

public class BeamController : MonoBehaviour
{
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _knockbackForce = 5f;
    private bool _canDamage = true;
    [SerializeField] private float _cooldownTimeAfterHit = 0.1f;

    void OnTriggerEnter(Collider other)
    {
        if (_canDamage && other.gameObject.CompareTag("Player") && !other.GetComponent<PlayerStateMachine>().IsDashing)
        {
            _canDamage = false;
            // Play SFX
            AudioManager.Instance.PlaySfx(AudioManager.Instance._playerHurtSharpSfx);

            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(_damage);
            playerHealth.DamageVfx.Play();

            // Get the point of impact
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            // Calculate the tangential direction at the point of impact
            Vector3 radiusVector = hitPoint - transform.position;
            Vector3 rotationAxis = Vector3.up; // Since we're rotating around Y axis
            Vector3 knockbackDirection = Vector3.Cross(rotationAxis, radiusVector).normalized;

            // Keep it on the horizontal plane
            knockbackDirection.y = 0f;


            // Apply the knockback through your player movement script
            PlayerStateMachine psm = other.gameObject.GetComponent<PlayerStateMachine>();
            if (psm != null)
            {
                psm.ApplyKnockback(knockbackDirection * _knockbackForce);
            }

            StartCoroutine(Cooldown());

        }
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(_cooldownTimeAfterHit);
        _canDamage = true;
    }
}
