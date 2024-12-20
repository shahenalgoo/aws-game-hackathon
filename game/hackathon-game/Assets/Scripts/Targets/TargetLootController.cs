using UnityEngine;

public class TargetLootController : MonoBehaviour
{
    [SerializeField] private float magneticRange = 5f; // Range at which magnetic effect starts
    [SerializeField] private float moveSpeed = 10f; // Speed at which item moves to player
    private Transform player;
    private bool isBeingPulled = false;

    [SerializeField] private ParticleSystem _lootCollectVFX;
    [SerializeField] private GameObject _trails;

    private void Start()
    {
        // Get reference to the player - assuming there's only one player with "Player" tag
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Once pulled, keep pulling regardless of distance
            if (!isBeingPulled && distanceToPlayer <= magneticRange)
            {
                isBeingPulled = true;

                // activate trails
                _trails.SetActive(true);

                // stop hovering
                GetComponent<Animator>().Play("Stop");
            }

            // If being pulled, continue pulling regardless of distance
            if (isBeingPulled)
            {
                Vector3 targetPosition = player.position + new Vector3(0, 1, 0); // Slightly above player
                transform.parent.LookAt(targetPosition);
                transform.parent.position = Vector3.MoveTowards(
                    transform.parent.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.IncrementLoot();

            // Spawn VFX
            _lootCollectVFX.transform.parent = null;
            _lootCollectVFX.Play();

            // Play SFX
            AudioManager.Instance.PlaySfx(AudioManager.Instance._starCollectedSfx);

            Destroy(gameObject);
        }
    }
}
