using System.Collections;
using UnityEngine;

public class RicochetSawBlade : MonoBehaviour
{

    [SerializeField] private int _damage = 20;
    [SerializeField] private float _knockbackForce = 1f;
    [SerializeField] private float _flySpeed = 5f;
    [SerializeField] private float _delayBeforeFlight = 3f;
    [SerializeField] private float _intervalBetweenFlight = 5f;

    private bool _canFly = false;
    private Vector3 _flyDirection;
    public Vector3 FlyDirection { get { return _flyDirection; } }
    private Transform _player;

    [SerializeField] private int _ricochetMax = 5;
    private int _ricochetCount = 0;

    [SerializeField] private Transform _homePosition;
    private bool _isHome = false;
    private bool _canGoHome = false;
    private float _homeReachedThreshold = 0.1f;

    private float _height;
    [SerializeField] private ParticleSystem _sparks;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _height = transform.parent.position.y;
        Invoke("StartFlight", 2f);
    }

    void StartFlight()
    {
        GetComponentInChildren<Animator>().Play("Spin");
        StartCoroutine(Fly());
    }

    void OnDisable()
    {
        CancelInvoke();
        StopCoroutine(Fly());
    }

    private IEnumerator Fly()
    {
        yield return new WaitForSeconds(_delayBeforeFlight);
        _canFly = true;
        _canGoHome = false;
        _ricochetCount = 0;
        _isHome = false;
        CalculateDirection();

        // Play sfx
        AudioManager.Instance.PlaySfx(AudioManager.Instance._sawBladeLaunchSfx);
    }

    void Update()
    {
        if (_canGoHome)
        {
            Vector3 targetPosition = new Vector3(_homePosition.position.x, _height, _homePosition.position.z);
            transform.parent.position = Vector3.MoveTowards(
                transform.parent.position,
                targetPosition,
                _flySpeed * Time.deltaTime
            );

            // Check if we've reached home
            if (!_isHome && Vector3.Distance(transform.parent.position, targetPosition) < _homeReachedThreshold)
            {
                _isHome = true;
                OnReachedHome();
            }
            return;
        }

        if (_canFly) transform.parent.position += _flyDirection * Time.deltaTime * _flySpeed;

    }

    void CalculateDirection()
    {
        _flyDirection = _player.position - transform.parent.position;
        _flyDirection.y = 0f;
        _flyDirection.Normalize();
    }

    public void OnReachedHome()
    {
        _canGoHome = false;
        _canFly = false;
        GetComponentInChildren<Animator>().Play("Stop");
        Invoke("StartFlight", _intervalBetweenFlight);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !other.GetComponent<PlayerStateMachine>().IsDashing)
        {
            // Play SFX
            AudioManager.Instance.PlaySfx(AudioManager.Instance._playerHurtSharpSfx);

            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(_damage);
            playerHealth.DamageVfx.Play();

            // Apply the knockback through your player movement script
            PlayerStateMachine psm = other.gameObject.GetComponent<PlayerStateMachine>();
            if (psm != null)
            {
                psm.ApplyKnockback(_flyDirection * _knockbackForce);
            }

            // Make blade go in calculated direction
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 normal = (hitPoint - other.transform.position).normalized;
            normal.y = 0f; // Keep it on the horizontal plane
            _flyDirection = normal;
            _flyDirection.Normalize();

        }

        if (other.gameObject.layer == LayerMask.NameToLayer("RicochetWall"))
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance._sawBladeRicochetSfx);

            Vector3 hitPoint = other.ClosestPoint(transform.position);
            _sparks.transform.position = hitPoint;
            _sparks.Play();


            _ricochetCount++;
            if (_ricochetCount <= _ricochetMax)
            {
                CalculateDirection();
            }
            else
            {
                // back home
                _canGoHome = true;
            }

        }

    }
}
