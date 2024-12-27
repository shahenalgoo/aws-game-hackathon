using System.Collections;
using TMPro;
using UnityEngine;

public class ExtractionController : MonoBehaviour
{
    [SerializeField] private float _flySpeed;
    [SerializeField] private float _waitTimeForDelayedActions;
    private bool _completionTriggered;
    private GameObject _player;
    private CharacterController _playerCC;

    [SerializeField] private GameObject _vfx;

    [SerializeField] private string _interactionText;

    // public void Start()
    // {
    //     Invoke("ActivatePlatform", 6f);
    // }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_player == null) _player = other.gameObject;
            PlayerStateMachine._interact += CompleteLevel;
            InteractTextController._setInteractionText(true, _interactionText);

            // bool isTutorial = TutorialManager.Instance != null;
            // bool hasCollectedAll = GameManager.Instance != null && GameManager.Instance.HasCollectedAll();

            // if (isTutorial || hasCollectedAll)
            // {
            //     PlayerStateMachine._interact += CompleteLevel;
            //     InteractTextController._setInteractionText(true, _interactionText);
            // }
            // else
            // {
            //     InteractTextController._setInteractionText(true, "Collect All Stars To Extract");
            // }
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
        if (!_completionTriggered) return;

        // Apply reverse gravity
        _playerCC.Move(Physics.gravity * Time.deltaTime * -1f * _flySpeed);
    }

    public void ActivatePlatform()
    {
        // Activate vfx
        _vfx.SetActive(true);

        // Play sfx
        AudioManager.Instance.PlaySfx(AudioManager.Instance._extractionReadySfx);

        // let player know on hud
        if (TutorialManager.Instance == null) HUDManager._noticeUpdater?.Invoke("The extraction platform is ready", 4f);

    }



    void CompleteLevel()
    {
        PlayerStateMachine psm = _player.GetComponent<PlayerStateMachine>();
        psm.HoverTornado.SetActive(true);
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

        // Play sfx
        AudioManager.Instance.PlaySfx(AudioManager.Instance._playerExtractSfx);

        // Stop time count
        GameManager.Instance?.StopTimeCount();

        StartCoroutine(TriggerDelayedActions());
    }

    private IEnumerator TriggerDelayedActions()
    {
        yield return new WaitForSeconds(_waitTimeForDelayedActions);
        // Make camera stop following
        CameraController.setCanFollow?.Invoke(false);

        // Make screen fade out
        UIManager.Instance._screenFader.GetComponent<Animator>().Play("Fade In");

        // Allo time to fade in
        yield return new WaitForSeconds(2f);

        // Go to Next Level
        GameManager.Instance?.GoToNextLevel();

        // In case of tutorial
        TutorialManager.Instance?.EndTutorial();
    }


}
