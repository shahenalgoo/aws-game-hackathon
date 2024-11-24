using UnityEngine;

public class LevelBuilder : MonoBehaviour
{

    [SerializeField]
    private GameObject[] levelObjects;

    private float yAdjustTarget = 0.35f;

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
        // create 2d array for loop
        for (int i = 0; i < instructions.GetLength(0); i++)
        {
            for (int j = 0; j < instructions.GetLength(1); j++)
            {
                int objectId = instructions[i, j];
                if (objectId == 0) continue;
                Vector3 pos = new Vector3(i, 0, j);

                GameObject obj = Instantiate(levelObjects[objectId], pos, Quaternion.identity);

                // If a target or bonus teleporter
                if (objectId == 2 || objectId == 7 || objectId == 8)
                {
                    // Add a floor underneath
                    Instantiate(levelObjects[1], pos, Quaternion.Euler(0, 90, 0));

                    // Adjust y of obj
                    obj.transform.position += new Vector3(0, yAdjustTarget, 0);
                }
            }
        }

    }
}
