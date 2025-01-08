using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 10f; // Degrees per second, can be adjusted in Inspector
    void Update()
    {
        // Rotate around the Y axis
        transform.Rotate(Vector3.up * (_rotationSpeed * Time.deltaTime));
    }
}
