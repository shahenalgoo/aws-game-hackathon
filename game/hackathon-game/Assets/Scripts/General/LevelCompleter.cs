using System.Threading.Tasks;
using UnityEngine;

public class LevelCompleter : MonoBehaviour
{
    [SerializeField] private float _flySpeed;
    [SerializeField] private float _cameraFollowTime;
    private bool _completionTriggered;
    private GameObject _player;
    private CharacterController _playerCC;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_player == null) _player = other.gameObject;

            if (GameManager.Instance.HasCollectedAll())
            {

                PlayerStateMachine._interact += CompleteLevel;
                InteractTextController._setInteractionText(true, "Press 'E' to Extract");
            }
            else
            {
                InteractTextController._setInteractionText(true, "Collect All Loot To Extract.");
            }
        }

    }

    void OnDisable()
    {
        PlayerStateMachine._interact -= CompleteLevel;
    }

    void OnTriggerExit(Collider other)
    {
        PlayerStateMachine._interact -= CompleteLevel;
        InteractTextController._setInteractionText(false, "");

    }

    void Update()
    {
        // Apply reverse gravity
        if (!_completionTriggered) return;

        _playerCC.Move(Physics.gravity * Time.deltaTime * -1f * _flySpeed);
    }

    void CompleteLevel()
    {
        PlayerStateMachine psm = _player.GetComponent<PlayerStateMachine>();
        psm.CharacterAnimator.SetBool("isReloading", false);
        psm.ToggleRigAndWeapon(false);
        psm.CanDash = false;
        psm.enabled = false;
        _player.GetComponentInChildren<GunManager>().gameObject.SetActive(false);
        _player.GetComponentInChildren<Canvas>().gameObject.SetActive(false);


        _player.GetComponent<Animator>().Play("Hovering");
        _playerCC = _player.GetComponent<CharacterController>();
        _playerCC.detectCollisions = false;
        _completionTriggered = true;
        Invoke("CameraStopsFollowPlayer", _cameraFollowTime);

        // Stop time count
        GameManager.Instance.CanCountTime = false;
    }

    void CameraStopsFollowPlayer()
    {
        // Make camera stop following
        CameraController.setCanFollow?.Invoke(false);
    }
}