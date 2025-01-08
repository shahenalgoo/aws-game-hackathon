using UnityEngine;

public class WorldUIRotationFix : MonoBehaviour
{
    private Quaternion _initialRotation;
    public void Start()
    {
        // Store the initial rotation we want to maintain
        _initialRotation = transform.rotation;
    }
    void LateUpdate()
    {
        transform.rotation = _initialRotation;
    }
}
