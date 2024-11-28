using UnityEngine;
using UnityEngine.UI;

public class CircularMinimap : MonoBehaviour
{
    [SerializeField] private RawImage minimapImage;
    [SerializeField] private Transform player; // Reference to player transform
    [SerializeField] private float radius = 50f; // Detection radius around player
    [SerializeField] private float minimapSize = 150f; // Size of the minimap in UI
    // [SerializeField] private float borderThickness = 5f;

    [SerializeField] private Color backgroundColor;

    private Texture2D minimapTexture;
    private int textureSize = 256; // Size of the texture (power of 2 is best for textures)

    void Start()
    {
        // Setup the UI image
        minimapImage.material = new Material(Shader.Find("UI/Default"));
        minimapImage.color = Color.white;

        // Create the circular mask
        CreateCircularMask();

        // Setup RectTransform for top-right positioning and proper scaling
        RectTransform rectTransform = minimapImage.GetComponent<RectTransform>();

        // Set size relative to screen height
        float size = Screen.height * 0.3f; // 20% of screen height, adjust this percentage as needed
        rectTransform.sizeDelta = new Vector2(size, size);

        // Position in top-right corner
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);

        // Offset from the corner (adjust these values as needed)
        rectTransform.anchoredPosition = new Vector2(-12, -12);
    }

    void CreateCircularMask()
    {
        minimapTexture = new Texture2D(textureSize, textureSize);
        minimapTexture.filterMode = FilterMode.Bilinear;

        float center = textureSize / 2f;
        float radius = textureSize / 2f;

        // Create circular mask with border
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));

                if (distance < radius)
                {
                    // Inside the circle (main minimap area)
                    minimapTexture.SetPixel(x, y, backgroundColor);
                }
                else
                {
                    // Outside the circle
                    minimapTexture.SetPixel(x, y, Color.clear);
                }
            }
        }

        minimapTexture.Apply();
        minimapImage.texture = minimapTexture;
    }


    void Update()
    {
        UpdateMinimap();
    }

    void UpdateMinimap()
    {
        if (player == null || minimapTexture == null) return;
        float center = textureSize / 2f;
        float rad = textureSize / 2f;
        // Clear the texture
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                if (distance < rad)
                {
                    minimapTexture.SetPixel(x, y, backgroundColor); // Changed to white background
                }
            }
        }

        // Get all relevant objects within radius
        Collider[] colliders = Physics.OverlapSphere(player.position, radius);

        // Calculate rotation values for 45 degrees clockwise
        float angle = 45f * Mathf.Deg2Rad; // Convert 45 degrees to radians
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        foreach (Collider collider in colliders)
        {
            // Get position relative to player
            Vector3 directionToObject = (collider.transform.position - player.position);

            // Rotate the position by 45 degrees clockwise
            Vector2 minimapPosition = new Vector2(
                directionToObject.x * cos + directionToObject.z * sin,   // Changed sign before sin
                -directionToObject.x * sin + directionToObject.z * cos   // Changed sign before sin
            );

            // Scale and center the position on the minimap
            minimapPosition = (minimapPosition / radius) * (textureSize / 2) + new Vector2(textureSize / 2, textureSize / 2);

            // Draw the object on the minimap
            if (collider.gameObject.tag == "MapItem")
            {
                DrawObjectOnMinimap(minimapPosition, Color.black, 11);
            }
        }

        // Draw player in the center
        Vector2 playerPos = new Vector2(textureSize / 2, textureSize / 2);
        DrawObjectOnMinimap(playerPos, Color.red, 5);

        minimapTexture.Apply();
    }

    void DrawObjectOnMinimap(Vector2 position, Color color, int size)
    {

        // Draw a diamond shape (rotated square)
        for (int y = -size; y <= size; y++)
        {
            // Calculate the width of this row of the diamond
            int rowWidth = size - Mathf.Abs(y);

            for (int x = -rowWidth; x <= rowWidth; x++)
            {
                int pixelX = Mathf.RoundToInt(position.x + x);
                int pixelY = Mathf.RoundToInt(position.y + y);

                // Check if the pixel is within texture bounds and within the circle
                if (pixelX >= 0 && pixelX < textureSize && pixelY >= 0 && pixelY < textureSize)
                {
                    float distanceFromCenter = Vector2.Distance(
                        new Vector2(pixelX, pixelY),
                        new Vector2(textureSize / 2, textureSize / 2)
                    );

                    if (distanceFromCenter <= textureSize / 2)
                    {
                        minimapTexture.SetPixel(pixelX, pixelY, color);
                    }
                }
            }
        }
    }

}
