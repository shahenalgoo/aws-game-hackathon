using System.Collections;
using UnityEngine;

public class PitfallController : MonoBehaviour
{

    private CharacterController _playerCC;
    [SerializeField] private ParticleSystem _onBoost;

    [SerializeField] private GameObject[] _safetyLedges; // 0 - left, 1 - top, 2 - right, 3 - down

    void Start()
    {
        // get grid pos from world pos
        Vector2Int gridPos = Helpers.GetGridPosition(transform);
        int[,] grid = LevelBuilder.Instance.Grid;


        // check if there's something on the left, activate safety ledge
        if (gridPos.y > 0 && grid[gridPos.x, gridPos.y - 1] != 0) _safetyLedges[0].SetActive(true);

        // check right
        if (gridPos.y < grid.GetLength(1) - 1 && grid[gridPos.x, gridPos.y + 1] != 0) _safetyLedges[2].SetActive(true);

        // check top
        if (gridPos.x > 0 && grid[gridPos.x - 1, gridPos.y] != 0) _safetyLedges[1].SetActive(true);

        // check bottom
        if (gridPos.x < grid.GetLength(0) - 1 && grid[gridPos.x + 1, gridPos.y] != 0) _safetyLedges[3].SetActive(true);

    }
    void Update()
    {
        if (_playerCC == null) return;

        // Apply gravity to player
        _playerCC.Move(Physics.gravity * Time.deltaTime);

    }

    public void PlayBooster()
    {
        _onBoost.Play();

        // Play sfx
        AudioManager.Instance.PlaySfx(AudioManager.Instance._dashBoosterSfx);
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
            playerHealth.Die(false);

            if (TutorialManager.Instance != null) StartCoroutine(RespawnPlayer());

        }

    }

    private IEnumerator RespawnPlayer()
    {
        // very dirty for now
        yield return new WaitForSeconds(1f);
        CameraController.setCanFollow?.Invoke(true);
        _playerCC.gameObject.GetComponent<Animator>().Play("Idle");
        _playerCC.transform.position = new Vector3(transform.position.x, 0, transform.position.z - 8f);
        _playerCC.detectCollisions = true;
        _playerCC.GetComponentInChildren<GunManager>(true).gameObject.SetActive(true);
        _playerCC.GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);

        PlayerHealth playerHealth = _playerCC.GetComponent<PlayerHealth>();
        playerHealth.IsDead = false;
        playerHealth.fallingTrailsObj.SetActive(false);

        PlayerStateMachine psm = _playerCC.gameObject.GetComponent<PlayerStateMachine>();
        psm.enabled = true;
        psm.CurrentState.SwitchState(psm._states.Idle());
        psm._dashLightning.gameObject.SetActive(true);
        psm.CanDash = true;
        if (psm.IsFightMode) psm.ToggleRigAndWeapon(true);
        psm.IsReloading = false;
        psm.ReloadAttempt = false;
        psm.IsShootingAllowed = true;

        ReloadBar._cancelReloadSlider?.Invoke();

        _playerCC = null;

    }
}
