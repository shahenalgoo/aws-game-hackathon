using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] private float _flySpeed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private bool _canFly;
    [SerializeField] private TrailRenderer _trail;


    private void OnEnable()
    {
        Invoke("DisableBullet", _lifeTime);
        _canFly = true;
    }

    void Update()
    {
        if (_canFly) transform.position += transform.forward * Time.deltaTime * _flySpeed;
    }

    public void DisableBullet()
    {
        _canFly = false;
        _trail.Clear();
        gameObject.SetActive(false);
    }
}
