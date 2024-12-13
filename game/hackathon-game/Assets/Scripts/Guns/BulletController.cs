using UnityEngine;

// The player's bullet controller
public class BulletController : Bullet
{

    // Or add this function for physics-based collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SawBlades"))
        {
            CancelInvoke("DisableBullet");
            BulletImpactManager._impactSpawner?.Invoke(transform.position, Quaternion.identity);
            DisableBullet();
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Detected");
        if (collision.gameObject.CompareTag("Target"))
        {
            CancelInvoke("DisableBullet");

            BulletImpactManager._impactSpawner?.Invoke(transform.position, Quaternion.identity);

            int damageRoundUp = Mathf.CeilToInt(_currentDamage);
            collision.gameObject.GetComponent<TargetHealth>().TakeDamage(damageRoundUp);

            DisableBullet();
        }
    }
}
