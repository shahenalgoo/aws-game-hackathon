using UnityEngine;

public enum MoveDirection
{
    Left,
    Right
}
public class SawBladeController : MonoBehaviour
{
    [SerializeField] private MoveDirection moveDirection;
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _knockbackForce = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Animator animator = GetComponentInChildren<Animator>();
        if (moveDirection == MoveDirection.Left)
        {
            animator.Play("MoveLeft");
        }
        else
        {
            animator.Play("MoveRight");

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
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