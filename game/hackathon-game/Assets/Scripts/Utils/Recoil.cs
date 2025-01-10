using UnityEngine;
using System.Collections;

// By Amazon Q
public class Recoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    [SerializeField] private float _recoilDistance = 0.5f;  // How far back the object will move
    [SerializeField] private float _recoilDuration = 0.1f;  // Time to move back
    [SerializeField] private float _returnDuration = 0.1f;  // Time to return to original position

    private Vector3 _originalPosition;
    private bool _isRecoiling = false;

    private void Start()
    {
        _originalPosition = transform.localPosition;
    }

    public void TriggerRecoil()
    {
        if (!_isRecoiling)
        {
            StartCoroutine(RecoilSequence());
        }
    }

    private IEnumerator RecoilSequence()
    {
        _isRecoiling = true;

        // Calculate recoil position (moving backward along local Z axis)
        Vector3 recoilPosition = _originalPosition - transform.forward * _recoilDistance;

        // Move back
        float elapsedTime = 0f;
        while (elapsedTime < _recoilDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _recoilDuration;
            transform.localPosition = Vector3.Lerp(_originalPosition, recoilPosition, t);
            yield return null;
        }

        // Return to original position
        elapsedTime = 0f;
        while (elapsedTime < _returnDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _returnDuration;
            transform.localPosition = Vector3.Lerp(recoilPosition, _originalPosition, t);
            yield return null;
        }

        // Ensure we end up exactly at the original position
        transform.localPosition = _originalPosition;
        _isRecoiling = false;
    }

    // Optional: Public properties to modify recoil parameters at runtime
    public float RecoilDistance
    {
        get => _recoilDistance;
        set => _recoilDistance = value;
    }

    public float RecoilDuration
    {
        get => _recoilDuration;
        set => _recoilDuration = value;
    }

    public float ReturnDuration
    {
        get => _returnDuration;
        set => _returnDuration = value;
    }
}
