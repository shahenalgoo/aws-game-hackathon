using System.Collections;
using UnityEngine;
// By Amazon Q
public class ExtractionPlatformFadeIn : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material[] materials;
    [SerializeField] private float fadeDuration = 2f;

    void Awake()
    {
        // Get the mesh renderer
        meshRenderer = GetComponent<MeshRenderer>();

        // Create material instances to avoid modifying the original materials
        materials = new Material[meshRenderer.materials.Length];
        for (int i = 0; i < meshRenderer.materials.Length; i++)
        {
            materials[i] = new Material(meshRenderer.materials[i]);
        }

        // Apply the material instances
        meshRenderer.materials = materials;

        // Set up materials for transparency
        SetupMaterialsForFade();

        // Initially make the object invisible
        SetAlpha(0);

        // Disable the renderer initially
        meshRenderer.enabled = false;

        // Disable collider
        GetComponent<CapsuleCollider>().enabled = false;
    }

    private void SetupMaterialsForFade()
    {
        foreach (Material material in materials)
        {
            // Setup material for transparency
            material.SetFloat("_Surface", 1f);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = 3000;

            // Ensure the material is set to transparent mode
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetShaderPassEnabled("ShadowCaster", false);
        }
    }

    private void SetAlpha(float alpha)
    {
        foreach (Material material in materials)
        {
            if (material.HasProperty("_BaseColor"))
            {
                Color color = material.GetColor("_BaseColor");
                color.a = alpha;
                material.SetColor("_BaseColor", color);
            }

            if (material.HasProperty("_Color"))
            {
                Color color = material.GetColor("_Color");
                color.a = alpha;
                material.SetColor("_Color", color);
            }
        }
    }

    public void StartFadeIn()
    {
        // Enable the renderer
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(3f);
        meshRenderer.enabled = true;

        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1f, elapsedTime / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        // Ensure we end at fully visible
        SetAlpha(1);

        // Reenable collider
        GetComponent<CapsuleCollider>().enabled = true;

        // Enable platform
        GetComponent<ExtractionController>().ActivatePlatform();
    }

    private void OnDestroy()
    {
        // Clean up material instances
        foreach (Material material in materials)
        {
            if (material != null)
            {
                Destroy(material);
            }
        }
    }
}
