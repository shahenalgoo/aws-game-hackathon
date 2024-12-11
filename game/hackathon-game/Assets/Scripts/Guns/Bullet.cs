using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _flySpeed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private float _damage;
    [SerializeField] private float _damageFalloffRate;
    [SerializeField] protected float _currentDamage;
    [SerializeField] private bool _canFly;
    [SerializeField] private TrailRenderer _trail;

    public void OnEnable()
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
        else
        {
            _currentDamage = 1f;
        }
    }

    public void DisableBullet()
    {
        _canFly = false;
        if (_trail != null) _trail.Clear();

        gameObject.SetActive(false);
    }
}
