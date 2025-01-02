using System.Collections;
using UnityEngine;

// move to position
// after countdown, crash to drop location
// reset
public class MissileController : MonoBehaviour
{
    private bool _readyPosReached = false;
    [SerializeField] private Vector3 _readyPosition;
    [SerializeField] private float _readyPosReachedThreshold = 0.1f;
    [SerializeField] private float _flySpeed = 5f;
    private Vector3 _dropPosition;
    public Vector3 DropPosition { get => _dropPosition; set => _dropPosition = value; }

    private bool _canRelease = false;
    [SerializeField] private float _delayToRelease = 2f;


    // Update is called once per frame
    void Update()
    {

        if (!_readyPosReached)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _readyPosition,
                _flySpeed * Time.deltaTime
            );

            transform.LookAt(_readyPosition);

            if (Vector3.Distance(transform.position, _readyPosition) < _readyPosReachedThreshold)
            {
                _readyPosReached = true;
                StartCoroutine(StartReleaseWithDelay());
            }
        }

        if (_canRelease)
        {
            transform.position = Vector3.MoveTowards(
                        transform.position,
                        _dropPosition,
                        _flySpeed * Time.deltaTime
                    );

            transform.LookAt(_dropPosition);
        }
    }

    private IEnumerator StartReleaseWithDelay()
    {
        yield return new WaitForSeconds(_delayToRelease);
        _canRelease = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }
}
