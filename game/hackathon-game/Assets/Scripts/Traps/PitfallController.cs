using UnityEngine;

public class PitfallController : MonoBehaviour
{

    private CharacterController _playerCC;
    [SerializeField] private ParticleSystem _onBoost;
    void Update()
    {
        if (_playerCC == null) return;

        // Apply gravity to player
        _playerCC.Move(Physics.gravity * Time.deltaTime);

    }

    public void PlayBooster()
    {
        _onBoost.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Make camera stop following
            CameraController.setCanFollow?.Invoke(false);

            _playerCC = other.GetComponent<CharacterController>();

            other.GetComponent<Animator>().Play("Falling");

            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            playerHealth.fallingTrailsObj.SetActive(true);
            other.GetComponent<PlayerHealth>().DisablePlayer();
        }

    }
}
