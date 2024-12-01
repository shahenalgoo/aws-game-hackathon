using UnityEngine;

public enum Shooter
{
    Player = 0,
    Target = 1

}

public class BulletController : MonoBehaviour
{
    [SerializeField] private float _flySpeed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private float _damage;
    [SerializeField] private float _damageFalloffRate;
    [SerializeField] private float _currentDamage;
    [SerializeField] private bool _canFly;
    [SerializeField] private TrailRenderer _trail;

    private Shooter _whoShot;


    private void OnEnable()
    {
        Invoke("DisableBullet", _lifeTime);
        _canFly = true;
        _currentDamage = _damage;
    }

    public void SetShooter(Shooter who)
    {
        _whoShot = who;
    }


    void Update()
    {
        if (_canFly) transform.position += transform.forward * Time.deltaTime * _flySpeed;


        if (_currentDamage > 1f)
        {
            _currentDamage -= _damageFalloffRate * Time.deltaTime;
        }
    }

    public void DisableBullet()
    {
        _canFly = false;
        _trail.Clear();
        gameObject.SetActive(false);
    }

    // Or add this function for physics-based collision
    private void OnTriggerEnter(Collider other)
    {
        if (_whoShot == Shooter.Player && other.gameObject.CompareTag("Target"))
        {
            int damageRoundUp = Mathf.CeilToInt(_currentDamage);
            other.gameObject.GetComponent<TargetHealth>().TakeDamage(damageRoundUp);
            CancelInvoke("DisableBullet");
            DisableBullet();
        }

        if (_whoShot == Shooter.Target && other.gameObject.CompareTag("Player"))
        {
            int damageRoundUp = Mathf.CeilToInt(_currentDamage);
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damageRoundUp);
            CancelInvoke("DisableBullet");
            DisableBullet();
        }



    }
}
