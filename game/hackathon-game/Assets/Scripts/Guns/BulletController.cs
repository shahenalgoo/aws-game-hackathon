using UnityEngine;

// The player's bullet controller
public class BulletController : Bullet
{

    // Or add this function for physics-based collision
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Target"))
        {
            int damageRoundUp = Mathf.CeilToInt(_currentDamage);
            other.gameObject.GetComponent<TargetHealth>().TakeDamage(damageRoundUp);

            OnBulletCollision();
            return;
        }
        if (other.gameObject.CompareTag("SawBlades") || other.gameObject.CompareTag("Turbine"))
        {
            OnBulletCollision();
            return;
        }

        if (other.gameObject.CompareTag("BossShield"))
        {
            OnBulletCollision();
            return;
        }

        if (other.gameObject.CompareTag("Boss"))
        {
            int damageRoundUp = Mathf.CeilToInt(_currentDamage);
            other.gameObject.GetComponent<BossHealth>().TakeDamage(damageRoundUp);
            OnBulletCollision();
            return;
        }


    }

    public void OnBulletCollision()
    {
        // Play sfx
        AudioManager.Instance.PlaySfx(AudioManager.Instance._targetHitSfx);

        CancelInvoke("DisableBullet");
        BulletImpactManager._impactSpawner?.Invoke(transform.position, Quaternion.identity);
        DisableBullet();
    }
}
