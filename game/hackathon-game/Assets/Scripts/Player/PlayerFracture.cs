using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerFracture : MonoBehaviour
{
    [Header("Fracture Settings")]
    [SerializeField] private int pieceCount = 15;
    [SerializeField] private float explosionForce = 300f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float upwardsModifier = 0.5f;

    [Header("Piece Settings")]
    [SerializeField] private float pieceMass = 1f;
    [SerializeField] private Material pieceMaterial;

    [Header("Trail Settings")]
    [SerializeField] private Material trailMaterial;
    [SerializeField] private float trailTime = 0.5f;
    [SerializeField] private float startWidth = 0.1f;
    [SerializeField] private float endWidth = 0.0f;
    [SerializeField] private Gradient trailColor;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDelay = 3f;
    [SerializeField] private float fadeDuration = 3f;

    [Header("Effects")]
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private AudioClip breakSound;

    private SkinnedMeshRenderer skinnedMeshRenderer;
    private List<GameObject> pieces = new List<GameObject>();

    // Store the original layer collision state
    private bool originalCollisionState;
    private int piecesLayer;
    private int invisibleWallLayer;

    private void Awake()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        // If no material assigned, use the original object's material
        if (pieceMaterial == null)
            pieceMaterial = skinnedMeshRenderer.material;

        // Store the layers we'll be working with
        piecesLayer = LayerMask.NameToLayer("Default"); // Or whatever layer your pieces use
        invisibleWallLayer = LayerMask.NameToLayer("InvisibleWall");

        // Store the original collision state between these layers
        originalCollisionState = Physics.GetIgnoreLayerCollision(piecesLayer, invisibleWallLayer);
    }

    private void OnDestroy()
    {
        // Restore the original collision state between layers
        Physics.IgnoreLayerCollision(piecesLayer, invisibleWallLayer, originalCollisionState);
    }

    public void Shatter()
    {
        // Bake the current pose of the skinned mesh into a new mesh
        Mesh bakedMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(bakedMesh);

        // Generate Voronoi points
        List<VoronoiPoint> voronoiPoints = GenerateVoronoiPoints(bakedMesh.bounds);

        // Get mesh data
        Vector3[] vertices = bakedMesh.vertices;
        int[] triangles = bakedMesh.triangles;
        Vector2[] uvs = bakedMesh.uv;

        // Assign vertices to nearest Voronoi point
        AssignVerticesToPoints(voronoiPoints, vertices, triangles, uvs);

        // Play effects
        PlayBreakEffects();

        // Create pieces
        CreatePieces(voronoiPoints);

        // Hide original object
        skinnedMeshRenderer.enabled = false;
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

            // Transform the point to world space
            Vector3 worldPoint = skinnedMeshRenderer.transform.TransformPoint(randomPoint);
            points.Add(new VoronoiPoint(worldPoint));
        }

        return points;
    }

    private void AssignVerticesToPoints(List<VoronoiPoint> points, Vector3[] vertices, int[] triangles, Vector2[] uvs)
    {
        // Process triangles in groups of three
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Calculate triangle center in world space
            Vector3 triangleCenter = (vertices[triangles[i]] +
                                    vertices[triangles[i + 1]] +
                                    vertices[triangles[i + 2]]) / 3f;

            Vector3 worldTriangleCenter = skinnedMeshRenderer.transform.TransformPoint(triangleCenter);

            // Find nearest point
            VoronoiPoint nearest = FindNearestPoint(worldTriangleCenter, points);

            // Add vertices and triangles to nearest point
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

    private void CreatePieces(List<VoronoiPoint> points)
    {
        foreach (VoronoiPoint point in points)
        {
            // Skip if not enough vertices
            if (point.Vertices.Count < 4) continue;

            // Check for unique vertices
            HashSet<Vector3> uniqueVertices = new HashSet<Vector3>(point.Vertices);
            if (uniqueVertices.Count < 4) continue;

            GameObject piece = new GameObject("Piece");
            piece.transform.position = skinnedMeshRenderer.transform.position;
            piece.transform.rotation = skinnedMeshRenderer.transform.rotation;

            // Add components
            MeshFilter meshFilter = piece.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = piece.AddComponent<MeshRenderer>();
            Rigidbody rb = piece.AddComponent<Rigidbody>();

            // Set layer and ignore collisions
            piece.layer = piecesLayer;
            Physics.IgnoreLayerCollision(piecesLayer, invisibleWallLayer, true);

            // Create mesh
            Mesh mesh = new Mesh();
            mesh.vertices = point.Vertices.ToArray();
            mesh.triangles = point.Triangles.ToArray();
            mesh.uv = point.UVs.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // Setup components
            meshFilter.mesh = mesh;
            Material instanceMaterial = new Material(pieceMaterial);
            SetupTransparentMaterial(instanceMaterial);
            meshRenderer.material = instanceMaterial;

            // Add collision with fallback options
            AddCollisionToPiece(piece, mesh);

            // Add trail renderer
            AddTrailRenderer(piece);

            // Setup rigidbody
            rb.mass = pieceMass;
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
            rb.AddTorque(Random.insideUnitSphere * explosionForce);

            pieces.Add(piece);

            // Start fade out coroutine
            StartCoroutine(FadeOutPiece(piece));
        }
    }

    private void AddCollisionToPiece(GameObject piece, Mesh originalMesh)
    {
        // Just use a sphere collider for maximum simplicity
        SphereCollider sphereCollider = piece.AddComponent<SphereCollider>();
        sphereCollider.radius = originalMesh.bounds.extents.magnitude * 0.5f;

        // Optional: Slightly randomize the radius for variety
        sphereCollider.radius *= Random.Range(0.8f, 1.2f);
    }

    private void AddTrailRenderer(GameObject piece)
    {
        TrailRenderer trail = piece.AddComponent<TrailRenderer>();

        // Basic trail settings
        trail.time = trailTime;
        trail.startWidth = startWidth;
        trail.endWidth = endWidth;
        trail.material = trailMaterial;

        // Set trail color gradient
        if (trailColor.colorKeys.Length == 0)
        {
            // Default gradient if none is set
            trailColor = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(Color.white, 0f);
            colorKeys[1] = new GradientColorKey(Color.white, 1f);

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            alphaKeys[1] = new GradientAlphaKey(0f, 1f);

            trailColor.SetKeys(colorKeys, alphaKeys);
        }
        trail.colorGradient = trailColor;

        // Additional trail settings
        trail.generateLightingData = true;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.receiveShadows = false;
        trail.autodestruct = false;
    }


    private IEnumerator FadeOutPiece(GameObject piece)
    {
        // Wait for fade delay
        yield return new WaitForSeconds(fadeDelay);

        MeshRenderer renderer = piece.GetComponent<MeshRenderer>();
        TrailRenderer trail = piece.GetComponent<TrailRenderer>();
        Material material = renderer.material;
        Color startColor = material.color;
        float elapsedTime = 0f;

        // Gradually fade out
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / fadeDuration;

            // Fade material
            Color newColor = new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                Mathf.Lerp(startColor.a, 0f, normalizedTime)
            );
            material.color = newColor;

            // Fade trail if it exists
            if (trail != null)
            {
                trail.startWidth = Mathf.Lerp(startWidth, 0f, normalizedTime);
                trail.endWidth = Mathf.Lerp(endWidth, 0f, normalizedTime);
            }

            yield return null;
        }

        // Destroy the piece after fading
        Destroy(piece);
    }

    // Make sure your material supports transparency
    private void SetupTransparentMaterial(Material material)
    {
        material.SetFloat("_Surface", 1f); // 0 = opaque, 1 = transparent
        material.SetFloat("_Blend", 0f);   // 0 = alpha, 1 = premultiply
        material.renderQueue = 3000;        // Transparent queue
        material.SetShaderPassEnabled("ShadowCaster", false);
    }


    private void PlayBreakEffects()
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }
    }
}

// VoronoiPoint class remains the same
public class VoronoiPoint
{
    public Vector3 Position { get; private set; }
    public List<Vector3> Vertices { get; private set; }
    public List<int> Triangles { get; private set; }
    public List<Vector2> UVs { get; private set; }

    public VoronoiPoint(Vector3 position)
    {
        Position = position;
        Vertices = new List<Vector3>();
        Triangles = new List<int>();
        UVs = new List<Vector2>();
    }
}


