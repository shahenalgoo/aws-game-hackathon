using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{

    private bool _phaseTwoActivated;
    private bool _phaseThreeActivated;
    public bool PhaseTwoActivated { get => _phaseTwoActivated; }
    public bool PhaseThreeActivated { get => _phaseThreeActivated; }

    [Header("Weapons")]
    [SerializeField] private BossTurret _turret;
    [SerializeField] private RicochetSawBlade _sawBlade;
    public RicochetSawBlade SawBlade { get => _sawBlade; }
    [SerializeField] private MissileLauncher _missileLauncher;

    [SerializeField] private Animator[] _shieldPieces;

    [SerializeField] private GameObject[] _laserBeams;

    public void ActivatePhaseTwo()
    {
        _phaseTwoActivated = true;
        Debug.Log("Phase 2 activated");

        _missileLauncher.gameObject.SetActive(true);
        _missileLauncher.MissileAmountPerAttack = 2;
    }

    public void ActivatePhaseThree()
    {
        _phaseThreeActivated = true;
        Debug.Log("Phase 3 activated");

        _turret.StopAttack();
        _turret.BurstAttackAmount = 1;
        _turret.StartRepeatingBurstAttack();


        StartCoroutine(FireLaserBeams());


        _missileLauncher.StopAttack();
        StartCoroutine(ActivateMissileLauncher());
    }

    private IEnumerator ActivateMissileLauncher()
    {
        yield return new WaitForSeconds(1f);
        _missileLauncher.MissileAmountPerAttack = 4;
        _missileLauncher.StartRepeatingAttack();

    }

    private IEnumerator FireLaserBeams()
    {
        for (int i = 0; i < _shieldPieces.Length; i++)
        {
            _shieldPieces[i].Play("Open");
        }

        yield return new WaitForSeconds(1.2f);

        for (int i = 0; i < _laserBeams.Length; i++)
        {
            _laserBeams[i].gameObject.SetActive(true);
        }
        // Play SFX
        AudioManager.Instance.PlaySfx(AudioManager.Instance._laserbeamFireSfx);
    }
}

// Phase 1
// Turret, Ricochet Blade

// Phase 2
// Turret, Ricochet Blade, 1 Missile 

// Phase 3
// Ricochet Blade, 4 missiles, laser beam, turret 1 bullet