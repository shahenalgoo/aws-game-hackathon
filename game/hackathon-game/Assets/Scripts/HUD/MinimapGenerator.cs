using UnityEngine;
using UnityEngine.UI;


// [RequireComponent(typeof(AspectRatioFitter))]
public class MinimapGenerator : MonoBehaviour
{
    [SerializeField] private RawImage minimapImage;
    private int width = 20;  // Your grid width
    private int height = 9;  // Your grid height
    private Texture2D minimapTexture;
    private int textureSize; // Size of the square texture
    private int[,] grid;

    public void Start()
    {
        // Add this at the start of your existing Start method
        minimapImage.material = new Material(Shader.Find("UI/Default"));
        minimapImage.color = Color.white; // Ensure full opacity on the RawImage itself

        // var aspectFitter = minimapImage.gameObject.GetComponent<AspectRatioFitter>();
        // if (aspectFitter == null)
        //     aspectFitter = minimapImage.gameObject.AddComponent<AspectRatioFitter>();

        // aspectFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        // aspectFitter.aspectRatio = 1f;

        grid = new int[,] {
            { 0, 0, 0, 2, 4, 1, 0, 0, 0, 6, 2, 0, 0, 0, 1, 5, 1, 0, 2, 7 },
            { 0, 1, 0, 1, 0, 2, 3, 1, 0, 3, 0, 0, 2, 1, 2, 0, 1, 2, 1, 0 },
            { 0, 2, 0, 0, 0, 0, 0, 1, 4, 1, 0, 0, 1, 0, 0, 0, 0, 5, 0, 0 },
            { 0, 1, 4, 2, 1, 0, 0, 0, 0, 2, 4, 1, 5, 0, 1, 3, 1, 2, 6, 0 },
            { 1, 2, 0, 0, 5, 1, 3, 6, 0, 0, 0, 0, 1, 4, 2, 0, 0, 0, 2, 1 },
            { 0, 0, 0, 0, 0, 0, 1, 5, 1, 5, 1, 0, 0, 0, 1, 3, 1, 0, 0, 0 },
            { 0, 1, 4, 2, 0, 0, 0, 0, 2, 0, 1, 3, 1, 0, 0, 0, 2, 1, 0, 8 },
            { 0, 0, 0, 3, 1, 2, 1, 0, 4, 0, 0, 0, 2, 4, 1, 0, 0, 5, 1, 1 },
            { 0, 0, 0, 1, 0, 0, 2, 5, 6, 3, 2, 0, 0, 0, 2, 1, 0, 0, 2, 0 },
        };

        GenerateMinimap(grid);
    }

    public void GenerateMinimap(int[,] levelData)
    {
        // Use the larger dimension for the square texture
        textureSize = Mathf.Max(width, height);
        minimapTexture = new Texture2D(textureSize, textureSize);
        minimapTexture.filterMode = FilterMode.Point;

        // Calculate offset to center the level data
        int offsetX = (textureSize - width) / 2;
        int offsetY = (textureSize - height) / 2;

        // First fill everything with transparent (background)
        Color transparentColor = new Color(0, 0, 0, 0); // Full transparency
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                minimapTexture.SetPixel(x, y, transparentColor);
            }
        }

        // Fill the texture based on level data
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (levelData[y, x] != 0)
                {
                    // Non-zero means path, color it black
                    minimapTexture.SetPixel(
                        x + offsetX,
                        height - 1 - y,  // Flip Y coordinates to match Unity's coordinate system
                        Color.black
                    );
                }
                else
                {
                    minimapTexture.SetPixel(
                        x + offsetX,
                        height - 1 - y,  // Flip Y coordinates to match Unity's coordinate system
                        Color.white
                    );
                }
            }
        }

        minimapTexture.Apply();

        // Assign the texture to the UI RawImage and set its size and position
        if (minimapImage != null)
        {
            minimapImage.texture = minimapTexture;

            // Make the minimap smaller (adjust these values as needed)
            RectTransform rectTransform = minimapImage.GetComponent<RectTransform>();
            // rectTransform.sizeDelta = new Vector2(200, 200); // Smaller size, adjust as needed

            // // Position in top-right corner (adjust these values as needed)
            // rectTransform.anchorMin = new Vector2(1, 1); // Top right anchor
            // rectTransform.anchorMax = new Vector2(1, 1);
            // rectTransform.pivot = new Vector2(1, 1); // Pivot at top right
            // rectTransform.anchoredPosition = new Vector2(-20, -20); // Offset from corner

            rectTransform.sizeDelta = new Vector2(200, 200);

            // Position in bottom-left corner
            rectTransform.anchorMin = new Vector2(0, 0); // Changed to bottom-left
            rectTransform.anchorMax = new Vector2(0, 0); // Changed to bottom-left
            rectTransform.pivot = new Vector2(0, 0);     // Changed to bottom-left
            rectTransform.anchoredPosition = new Vector2(20, 20); // Positive offset to move away from corner
        }
    }

}
