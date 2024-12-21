using UnityEngine;

public class TargetMeleeController : MonoBehaviour
{
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _knockbackForce = 6f;
    private TargetController _targetController;

    public void Start()
    {
        _targetController = GetComponentInParent<TargetController>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (_targetController.MeleeAnimator.GetCurrentAnimatorStateInfo(0).IsName("SpinAttack") && other.gameObject.CompareTag("Player") && !other.gameObject.GetComponent<PlayerStateMachine>().IsDashing)
        {
            // Play sfx
            AudioManager.Instance.PlaySfx(AudioManager.Instance._playerHurtMeleeSfx);

            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(_damage);
            playerHealth.DamageVfx.Play();

            // Add knockback using CharacterController
            CharacterController playerCC = other.gameObject.GetComponent<CharacterController>();
            if (playerCC != null)
            {
                Vector3 knockbackDirection = transform.forward;

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
