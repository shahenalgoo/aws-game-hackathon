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
    [SerializeField] private GameObject _shield;
    [SerializeField] private Animator[] _shieldPieces;
    [SerializeField] private GameObject[] _laserBeams;



    public void ActivatePhaseTwo()
    {
        _phaseTwoActivated = true;
        _missileLauncher.gameObject.SetActive(true);
        _missileLauncher.MissileAmountPerAttack = 2;
    }

    public void ActivatePhaseThree()
    {
        _phaseThreeActivated = true;

        _turret.StopAttack();
        _turret.BurstAttackAmount = 2;
        _turret.StartRepeatingBurstAttack();

        StartCoroutine(FireLaserBeams());

        _missileLauncher.StopAttack();
        StartCoroutine(ActivateMissileLauncher());
    }

    private IEnumerator ActivateMissileLauncher()
    {
        yield return new WaitForSeconds(1f);

        if (_missileLauncher == null)
        {
            yield break;
        }
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
            if (_laserBeams[i] == null)
            {
                yield break;
            }
            _laserBeams[i].gameObject.SetActive(true);
        }
        // Play SFX
        AudioManager.Instance.PlaySfx(AudioManager.Instance._laserbeamFireSfx);
    }

    public void Explode()
    {
        // Turret
        _turret.StopAttack();

        // Ricochet blade
        _sawBlade.enabled = false;

        // Missiles
        _missileLauncher.StopAttack();
        _missileLauncher.enabled = false;

        // Lasers
        for (int i = 0; i < _laserBeams.Length; i++)
        {
            _laserBeams[i].gameObject.SetActive(false);
        }

        // Shield
        _shield.GetComponent<Rotator>().enabled = false;

        // Play sfx
        AudioManager.Instance.PlaySfx(AudioManager.Instance._bossDeathSfx);

        // Shatter everything
        GetComponent<BossFracture>().Shatter();

        // Destroy all children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }


}

// Phase 1
// Turret, Ricochet Blade

// Phase 2
// Turret, Ricochet Blade, 1 Missile 

// Phase 3
// Ricochet Blade, 4 missiles, laser beam, turret 2 bullet