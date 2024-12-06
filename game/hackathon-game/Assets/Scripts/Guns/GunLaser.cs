using UnityEngine;

public class GunLaser : MonoBehaviour
{
    private LineRenderer laserLine;
    public float laserLength = 50f;
    public float laserWidth = 0.02f;
    public Color laserColor = Color.red;
    private LayerMask layerMask;
    private bool isLaserEnabled = true;

    void Start()
    {
        laserLine = gameObject.AddComponent<LineRenderer>();
        laserLine.startWidth = laserWidth;
        laserLine.endWidth = laserWidth;
        laserLine.material = new Material(Shader.Find("Sprites/Default"));
        laserLine.startColor = laserColor;
        laserLine.endColor = laserColor;
        laserLine.positionCount = 2;

        // Create a layer mask that ignores both InvisibleWall and Particle layers
        int invisibleWallLayer = LayerMask.NameToLayer("InvisibleWall");
        int dashBoosterLayer = LayerMask.NameToLayer("Particles");

        // Create a layer mask that ignores the InvisibleWall layer
        // layerMask = ~(1 << LayerMask.NameToLayer("InvisibleWall"));
        layerMask = ~((1 << invisibleWallLayer) | (1 << dashBoosterLayer));
    }

    void Update()
    {
        if (!isLaserEnabled)
        {
            laserLine.enabled = false;
            // Set both positions to the origin point to clear the laser
            laserLine.SetPosition(0, Vector3.zero);
            laserLine.SetPosition(1, Vector3.zero);
            return;
        }

        laserLine.enabled = true;

        Vector3 startPosition = transform.position - (transform.right * 0.25f);
        Vector3 endPosition;

        RaycastHit hit;
        if (Physics.Raycast(startPosition, transform.right, out hit, laserLength, layerMask))
        {
            endPosition = hit.point;
        }
        else
        {
            endPosition = startPosition + transform.right * laserLength;
        }

        laserLine.SetPosition(0, startPosition);
        laserLine.SetPosition(1, endPosition);
    }

    public void EnableLaser(bool value)
    {
        isLaserEnabled = value;
    }
}

