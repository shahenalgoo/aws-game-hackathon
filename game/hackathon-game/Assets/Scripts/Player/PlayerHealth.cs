using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // For URP

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth;

    [Header("Damage Effect")]
    [SerializeField] private Volume damageVolume;
    [SerializeField] private float flashDuration = 0.2f;
    private Vignette vignette;


    [SerializeField] private float explosionForce = 300f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int numberOfPieces = 10;

    private MeshFilter meshFilter;
    private List<GameObject> pieces = new List<GameObject>();
    public void Start()
    {
        _currentHealth = _maxHealth;

        // Set max health
        HUDManager._maxHealthUpdater?.Invoke(_currentHealth);

        // Get the vignette effect from the volume
        if (damageVolume != null && damageVolume.profile.TryGet(out vignette))
        {
            // Initialize vignette settings
            vignette.intensity.Override(0f);
            vignette.color.Override(Color.red);
        }

        meshFilter = GetComponent<MeshFilter>();
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        // Update the health bar
        HUDManager._targetHealthUpdater?.Invoke(_currentHealth);

        // Trigger damage effect
        StartCoroutine(FlashRed());

        // Play hurt animation
        PlayerStateMachine psm = GetComponent<PlayerStateMachine>();
        if (!psm.IsStunned) psm.IsStunned = true;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        if (vignette == null) yield break;

        float elapsedTime = 0f;
        float startIntensity = 0f;
        float targetIntensity = 0.5f;

        // Fade in
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float newIntensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / (flashDuration / 2));
            vignette.intensity.Override(newIntensity);
            yield return null;
        }

        // Fade out
        elapsedTime = 0f;
        startIntensity = targetIntensity;
        targetIntensity = 0f;

        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float newIntensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / (flashDuration / 2));
            vignette.intensity.Override(newIntensity);
            yield return null;
        }

        vignette.intensity.Override(0f);
    }


    private void Die()
    {
        // Store original mesh data
        Mesh originalMesh = meshFilter.mesh;
        Vector3[] vertices = originalMesh.vertices;
        int[] triangles = originalMesh.triangles;
        Vector2[] uvs = originalMesh.uv;

        // Create chunks
        for (int i = 0; i < numberOfPieces; i++)
        {
            GameObject piece = new GameObject("Piece_" + i);
            piece.transform.position = transform.position;
            piece.transform.rotation = transform.rotation;

            // Add mesh components
            MeshFilter pieceFilter = piece.AddComponent<MeshFilter>();
            MeshRenderer pieceRenderer = piece.AddComponent<MeshRenderer>();
            MeshCollider pieceCollider = piece.AddComponent<MeshCollider>();
            Rigidbody pieceRigidbody = piece.AddComponent<Rigidbody>();

            // Copy material
            pieceRenderer.material = GetComponent<MeshRenderer>().material;

            // Create submesh
            // This is a simplified version - you'd want more sophisticated mesh splitting
            Mesh pieceMesh = new Mesh();
            // ... mesh splitting logic here

            pieceFilter.mesh = pieceMesh;
            pieceCollider.convex = true;

            // Add explosion force
            Vector3 randomDir = Random.insideUnitSphere;
            pieceRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            pieceRigidbody.AddTorque(randomDir * explosionForce);

            pieces.Add(piece);
        }

        // Hide original robot
        gameObject.SetActive(false);

        // Die visuals & audio feedback

        // Remove this later
        // Destroy(gameObject);

        // Death screen

    }
}
