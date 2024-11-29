using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] private float _flySpeed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private float _damage;
    [SerializeField] private float _damageFalloffRate;
    [SerializeField] private float _currentDamage;
    [SerializeField] private bool _canFly;
    [SerializeField] private TrailRenderer _trail;

    private void OnEnable()
    {
        Invoke("DisableBullet", _lifeTime);
        _canFly = true;
        _currentDamage = _damage;
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
        if (other.gameObject.CompareTag("Target"))
        {
            int damageRoundUp = Mathf.CeilToInt(_currentDamage);
            other.gameObject.GetComponent<TargetController>().TakeDamage(damageRoundUp);
        }

        CancelInvoke("DisableBullet");
        DisableBullet();
    }
}
