using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// This script fractures all MeshRenderers in children
// By Amazon Q
public class BossFracture : MonoBehaviour
{
    [Header("Fracture Settings")]
    [SerializeField] private int pieceCount = 15;
    [SerializeField] private float explosionForce = 300f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float upwardsModifier = 0.5f;

    [Header("Piece Settings")]
    [SerializeField] private float pieceMass = 1f;
    // [SerializeField] private Material pieceMaterial;

    [Header("Trail Settings")]
    [SerializeField] private Material trailMaterial;
    [SerializeField] private float trailTime = 0.5f;
    [SerializeField] private float startWidth = 0.1f;
    [SerializeField] private float endWidth = 0.0f;
    [SerializeField] private Gradient trailColor;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDelay = 3f;
    [SerializeField] private float fadeDuration = 3f;

    private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    private List<GameObject> pieces = new List<GameObject>();

    [Header("Layer Collision Settings")]
    private Dictionary<int, bool> originalCollisionStates = new Dictionary<int, bool>();
    private int piecesLayer;
    [SerializeField] private LayerMask layersToIgnore;

    private void Awake()
    {
        // Get all MeshRenderers in children
        meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>());

        piecesLayer = LayerMask.NameToLayer("FracturePieces");

        // Store original collision states
        for (int i = 0; i < 32; i++)
        {
            if ((layersToIgnore.value & (1 << i)) != 0)
            {
                originalCollisionStates[i] = Physics.GetIgnoreLayerCollision(piecesLayer, i);
            }
        }
    }

    private void OnDestroy()
    {
        // Restore original collision states
        foreach (var layerState in originalCollisionStates)
        {
            Physics.IgnoreLayerCollision(piecesLayer, layerState.Key, layerState.Value);
        }
    }
    public void Shatter()
    {

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            // Get the MeshFilter component
            MeshFilter meshFilter = meshRenderer.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.mesh == null) continue;

            Mesh originalMesh = meshFilter.mesh;

            // Generate Voronoi points
            List<VoronoiPoint> voronoiPoints = GenerateVoronoiPoints(originalMesh.bounds);

            // Get mesh data
            Vector3[] vertices = originalMesh.vertices;
            int[] triangles = originalMesh.triangles;
            Vector2[] uvs = originalMesh.uv;

            // Assign vertices to nearest Voronoi point
            AssignVerticesToPoints(voronoiPoints, vertices, triangles, uvs, meshRenderer.transform);

            // Create pieces
            CreatePieces(voronoiPoints, meshRenderer);

            // Hide original object
            meshRenderer.enabled = false;
        }
    }

    private List<VoronoiPoint> GenerateVoronoiPoints(Bounds bounds)
    {
        List<VoronoiPoint> points = new List<VoronoiPoint>();

        for (int i = 0; i < pieceCount; i++)
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            points.Add(new VoronoiPoint(randomPoint));
        }

        return points;
    }

    private void AssignVerticesToPoints(List<VoronoiPoint> points, Vector3[] vertices, int[] triangles, Vector2[] uvs, Transform meshTransform)
    {
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 triangleCenter = (vertices[triangles[i]] +
                                    vertices[triangles[i + 1]] +
                                    vertices[triangles[i + 2]]) / 3f;

            Vector3 worldTriangleCenter = meshTransform.TransformPoint(triangleCenter);

            VoronoiPoint nearest = FindNearestPoint(worldTriangleCenter, points);

            int vertexOffset = nearest.Vertices.Count;

            for (int j = 0; j < 3; j++)
            {
                nearest.Vertices.Add(vertices[triangles[i + j]]);
                nearest.Triangles.Add(vertexOffset + j);
                nearest.UVs.Add(uvs[triangles[i + j]]);
            }
        }
    }

    private VoronoiPoint FindNearestPoint(Vector3 position, List<VoronoiPoint> points)
    {
        VoronoiPoint nearest = points[0];
        float minDistance = Vector3.Distance(position, nearest.Position);

        for (int i = 1; i < points.Count; i++)
        {
            float distance = Vector3.Distance(position, points[i].Position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = points[i];
            }
        }

        return nearest;
    }

    private void CreatePieces(List<VoronoiPoint> points, MeshRenderer originalRenderer)
    {
        foreach (VoronoiPoint point in points)
        {
            if (point.Vertices.Count < 4) continue;

            HashSet<Vector3> uniqueVertices = new HashSet<Vector3>(point.Vertices);
            if (uniqueVertices.Count < 4) continue;

            GameObject piece = new GameObject("Piece");
            piece.transform.position = originalRenderer.transform.position;
            piece.transform.rotation = originalRenderer.transform.rotation;
            piece.transform.localScale = originalRenderer.transform.localScale;

            MeshFilter meshFilter = piece.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = piece.AddComponent<MeshRenderer>();
            Rigidbody rb = piece.AddComponent<Rigidbody>();

            piece.layer = piecesLayer;

            for (int i = 0; i < 32; i++)
            {
                if ((layersToIgnore.value & (1 << i)) != 0)
                {
                    Physics.IgnoreLayerCollision(piecesLayer, i, true);
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = point.Vertices.ToArray();
            mesh.triangles = point.Triangles.ToArray();
            mesh.uv = point.UVs.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshFilter.mesh = mesh;

            // Copy all materials from the original renderer
            Material[] materials = new Material[originalRenderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = new Material(originalRenderer.materials[i]);
                SetupTransparentMaterial(materials[i]);
            }
            meshRenderer.materials = materials;

            AddCollisionToPiece(piece, mesh);
            AddTrailRenderer(piece);

            rb.mass = pieceMass;
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);

            pieces.Add(piece);
            StartCoroutine(FadeOutPiece(piece));
        }
    }

    private void AddCollisionToPiece(GameObject piece, Mesh originalMesh)
    {
        SphereCollider sphereCollider = piece.AddComponent<SphereCollider>();
        sphereCollider.radius = originalMesh.bounds.extents.magnitude * 0.5f;
        sphereCollider.radius *= Random.Range(0.8f, 1.2f);
    }

    private void AddTrailRenderer(GameObject piece)
    {
        GameObject trailObject = new GameObject("TrailPoint");
        trailObject.transform.SetParent(piece.transform);

        MeshFilter meshFilter = piece.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.mesh != null)
        {
            trailObject.transform.localPosition = meshFilter.mesh.bounds.center;
        }

        TrailRenderer trail = trailObject.AddComponent<TrailRenderer>();
        trail.material = trailMaterial;
        trail.time = trailTime;
        trail.startWidth = startWidth;
        trail.endWidth = endWidth;
        trail.colorGradient = trailColor;
    }

    private void SetupTransparentMaterial(Material material)
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

    private IEnumerator FadeOutPiece(GameObject piece)
    {
        yield return new WaitForSeconds(fadeDelay);

        MeshRenderer renderer = piece.GetComponent<MeshRenderer>();
        Material[] materials = renderer.materials;
        Color[] originalColors = new Color[materials.Length];

        // Store original colors
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            // Update alpha for all materials
            for (int i = 0; i < materials.Length; i++)
            {
                Color originalColor = originalColors[i];
                materials[i].color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }

            yield return null;
        }

        Destroy(piece);
    }
}

