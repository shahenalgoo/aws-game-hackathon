using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField]
    private int tileSize = 2;

    [SerializeField]
    private GameObject[] levelObjects;

    private float yAdjustTarget = 1f;

    private int[,] instructions;


    // Start is called before the first frame update
    void Start()
    {
        instructions = new int[,] {
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

        BuildLevel();
    }

    void BuildLevel()
    {
        GridResolver resolver = new GridResolver(instructions);
        int[,] fixedInstructions = resolver.FixIsolatedRegions(4, 0);

        // create 2d array for loop
        for (int i = 0; i < fixedInstructions.GetLength(0); i++)
        {
            for (int j = 0; j < fixedInstructions.GetLength(1); j++)
            {
                int objectId = fixedInstructions[i, j];
                if (objectId == 0) continue;
                Vector3 pos = new Vector3(i * tileSize, -5f, j * tileSize);

                GameObject obj = Instantiate(levelObjects[objectId], pos, Quaternion.identity);

                // If a target or bonus teleporter
                if (objectId == 2 || objectId == 7 || objectId == 8)
                {
                    // Add a floor underneath
                    GameObject floor = Instantiate(levelObjects[1], pos, Quaternion.identity);

                    // Adjust y of obj
                    obj.transform.position = new Vector3(pos.x, yAdjustTarget, pos.z);
                }
            }
        }
    }
}
