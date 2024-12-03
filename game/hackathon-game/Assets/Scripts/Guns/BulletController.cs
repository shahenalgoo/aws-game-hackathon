using UnityEngine;

// The player's bullet controller
public class BulletController : Bullet
{

    // Or add this function for physics-based collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            CancelInvoke("DisableBullet");

            BulletImpactManager._impactSpawner?.Invoke(transform.position, Quaternion.identity);

            int damageRoundUp = Mathf.CeilToInt(_currentDamage);
            other.gameObject.GetComponent<TargetHealth>().TakeDamage(damageRoundUp);

            DisableBullet();
        }
    }
}
