using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtractionController : MonoBehaviour
{
    [SerializeField] private float _flySpeed;
    [SerializeField] private float _waitTimeForDelayedActions;
    private bool _completionTriggered;
    private GameObject _player;
    private CharacterController _playerCC;

    [SerializeField] private GameObject _vfx;

    [SerializeField] private string _interactionText;

    [DllImport("__Internal")]
    private static extern void PlayVoiceline(string type);

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_player == null) _player = other.gameObject;

            bool isTutorial = TutorialManager.Instance != null;
            bool hasCollectedAll = GameManager.Instance != null && GameManager.Instance.HasCollectedAll();

            if (isTutorial || hasCollectedAll)
            {
                PlayerStateMachine._interact += CompleteLevel;
                InteractTextController._setInteractionText(true, _interactionText);
            }
            else
            {
                InteractTextController._setInteractionText(true, "Collect All Stars To Extract");
            }
        }

    }

    void OnDisable()
    {
        PlayerStateMachine._interact -= CompleteLevel;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerStateMachine._interact -= CompleteLevel;
            InteractTextController._setInteractionText(false, "");
        }
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
        if (TutorialManager.Instance == null) HUDManager._noticeUpdater?.Invoke("The extraction platform is ready", 3f);

        // If boss fight, request voiceline with a delay
        if (SceneManager.GetActiveScene().buildIndex == SceneIndexes.BossFightSceneIndex) StartCoroutine(RequestDeathVoiceline());
    }

    private IEnumerator RequestDeathVoiceline()
    {
        yield return new WaitForSeconds(2f);
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        PlayVoiceline("afterBossFight");
#endif
        Debug.Log("Boss died, voiceline requested");
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
        CameraController._setCanFollow?.Invoke(false);

        // Make screen fade 
        UIManager.Instance._screenFader.GetComponent<Animator>().Play("Fade In");
        HUDManager.Instance?.TriggerFadeOut();

        // Allo time to fade in
        yield return new WaitForSeconds(2f);

        // Go to Next Level
        GameManager.Instance?.GoToNextLevel();

        // In case of tutorial
        TutorialManager.Instance?.EndTutorial();
    }


}
