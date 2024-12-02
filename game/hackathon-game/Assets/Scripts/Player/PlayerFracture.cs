using UnityEngine;
using System.Collections.Generic;

public class PlayerFracture : MonoBehaviour
{
    [Header("Fracture Settings")]
    [SerializeField] private int pieceCount = 15;
    [SerializeField] private float explosionForce = 300f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float upwardsModifier = 0.5f;

    [Header("Piece Settings")]
    [SerializeField] private float pieceMass = 1f;
    [SerializeField] private float pieceLifetime = 3f;
    [SerializeField] private Material pieceMaterial;

    [Header("Effects")]
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private AudioClip breakSound;

    private SkinnedMeshRenderer skinnedMeshRenderer;
    private List<GameObject> pieces = new List<GameObject>();

    private void Awake()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        // If no material assigned, use the original object's material
        if (pieceMaterial == null)
            pieceMaterial = skinnedMeshRenderer.material;
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

        // Create pieces
        CreatePieces(voronoiPoints);

        // Play effects
        PlayBreakEffects();

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
            if (point.Vertices.Count == 0) continue;

            GameObject piece = new GameObject("Piece");
            piece.transform.position = skinnedMeshRenderer.transform.position;
            piece.transform.rotation = skinnedMeshRenderer.transform.rotation;

            // Add components
            MeshFilter meshFilter = piece.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = piece.AddComponent<MeshRenderer>();
            MeshCollider meshCollider = piece.AddComponent<MeshCollider>();
            Rigidbody rb = piece.AddComponent<Rigidbody>();

            // Create mesh
            Mesh mesh = new Mesh();
            mesh.vertices = point.Vertices.ToArray();
            mesh.triangles = point.Triangles.ToArray();
            mesh.uv = point.UVs.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // Setup components
            meshFilter.mesh = mesh;
            meshRenderer.material = pieceMaterial;
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true;

            // Setup rigidbody
            rb.mass = pieceMass;
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
            rb.AddTorque(Random.insideUnitSphere * explosionForce);

            pieces.Add(piece);

            // Destroy piece after lifetime
            Destroy(piece, pieceLifetime);
        }
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
