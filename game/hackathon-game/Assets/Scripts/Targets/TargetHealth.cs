using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHealth : MonoBehaviour
{
    // Add these variables to your existing ones
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth;
    private bool _isDead;
    [SerializeField] private HealthBar healthBar;

    [SerializeField] private float _fadeOutDuration = 2f;

    [SerializeField] private GameObject _deathVfxObj;


    public void Start()
    {
        _currentHealth = _maxHealth;

        // Spawn the health bar
        CreateHealthBar();
    }

    private void CreateHealthBar()
    {
        // Set the initial max health
        healthBar.SetMaxHealth(_maxHealth);
    }
    public void TakeDamage(int amount)
    {
        if (_isDead) return;

        _currentHealth -= amount;

        // Update the health bar
        healthBar.UpdateHealthBar(_currentHealth);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // set bool
        _isDead = true;

        // turn off health bar
        healthBar.gameObject.SetActive(false);

        // stop interacting with player
        TargetController targetController = GetComponent<TargetController>();
        targetController.Player = null;

        // rotate head down
        targetController.TargetBarrel.GetComponent<Animator>().Play("HeadDown");

        // Disable all colliders
        var colliders = GetComponentsInChildren<BoxCollider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        // Play VFX
        _deathVfxObj.SetActive(true);

        // Play SFX
        AudioManager.Instance.PlaySfx(AudioManager.Instance._targetExplodeSfx);

        // Fade out obj
        StartCoroutine(FadeOut());

        // Spawn loot
        Vector3 lootSpawnPos = transform.position + new Vector3(0, 1f, 0);
        if (GameManager.Instance != null) Instantiate(GameManager.Instance.TargetLoot, lootSpawnPos, Quaternion.Euler(0, -45f, 0));
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        // Get all mesh renderers
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        // Create a list to store all materials and their original colors
        List<Material> allMaterials = new List<Material>();
        List<Color> originalColors = new List<Color>();

        // Collect all materials from all mesh renderers
        foreach (MeshRenderer renderer in meshRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                allMaterials.Add(material);
                originalColors.Add(material.color);

                // Setup material for transparency
                material.SetFloat("_Surface", 1f);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                material.renderQueue = 3000;
            }
        }

        // Fade out loop
        while (elapsedTime < _fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / _fadeOutDuration;

            // Update all materials
            for (int i = 0; i < allMaterials.Count; i++)
            {
                Color newColor = originalColors[i];
                newColor.a = Mathf.Lerp(1f, 0f, normalizedTime);
                allMaterials[i].color = newColor;
            }

            yield return null;
        }

        // Ensure all materials end up fully transparent
        for (int i = 0; i < allMaterials.Count; i++)
        {
            Color finalColor = originalColors[i];
            finalColor.a = 0f;
            allMaterials[i].color = finalColor;
        }

        // Destroy the object after fade
        Destroy(gameObject);
    }

}
