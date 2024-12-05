using UnityEngine;

public class PitfallController : MonoBehaviour
{

    private CharacterController _playerCC;
    void Update()
    {
        if (_playerCC == null) return;

        // Apply gravity to player
        _playerCC.Move(Physics.gravity * Time.deltaTime);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Make camera stop following
            CameraController.setCanFollow?.Invoke(false);

            _playerCC = other.GetComponent<CharacterController>();

            _playerCC.GetComponent<Animator>().Play("Falling");

            other.GetComponent<PlayerHealth>().DisablePlayer();
        }

    }
}
