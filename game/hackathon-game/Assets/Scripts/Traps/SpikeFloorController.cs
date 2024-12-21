using System.Collections;
using UnityEngine;

public class SpikeFloorController : MonoBehaviour
{
    [SerializeField] private float _sfxStartDelay;
    private Animator _spikes;
    [SerializeField] private float _detectionColliderHeightOnTrigger = 5f;
    [SerializeField] private float _detectionColliderReturnTime = 3f;

    public void Start()
    {
        _spikes = transform.parent.GetComponentInChildren<Animator>();
    }

    public void TriggerSpikes()
    {
        _spikes.Play("SpikePlay");
        StartCoroutine(PlaySpikeSFX());
    }

    private IEnumerator PlaySpikeSFX()
    {
        yield return new WaitForSeconds(_sfxStartDelay);
        // Play sfx
        AudioManager.Instance.PlaySfx(AudioManager.Instance._spikeTrapSfx);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            transform.localPosition = new Vector3(0, _detectionColliderHeightOnTrigger, 0);
            TriggerSpikes();
            StartCoroutine(BringBackDetectionCollider(_detectionColliderReturnTime));
        }
    }

    private IEnumerator BringBackDetectionCollider(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        transform.localPosition = Vector3.zero;

    }

}
