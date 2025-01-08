using System.Collections.Generic;

// By Amazon Q
public class GridResolver
{
    private int[,] grid;
    private int[,] regions;
    private int rows;
    private int cols;
    private HashSet<int> reachableRegions;

    public GridResolver(int[,] instructions)
    {
        grid = instructions;
        rows = grid.GetLength(0);
        cols = grid.GetLength(1);
        regions = new int[rows, cols];
        reachableRegions = new HashSet<int>();
    }

    public int[,] FixIsolatedRegions(int startRow, int startCol)
    {
        // Create a copy of the original grid
        int[,] fixedGrid = (int[,])grid.Clone();

        // First pass: Label all connected regions
        int regionCounter = 1;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (fixedGrid[i, j] != 0 && regions[i, j] == 0)
                {
                    LabelRegion(i, j, regionCounter);
                    regionCounter++;
                }
            }
        }

        // Mark the region containing the start position as reachable
        if (fixedGrid[startRow, startCol] != 0)
        {
            reachableRegions.Add(regions[startRow, startCol]);
        }

        // Find all isolated regions
        Dictionary<int, List<(int row, int col)>> isolatedRegions = new Dictionary<int, List<(int row, int col)>>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (fixedGrid[i, j] != 0 && !reachableRegions.Contains(regions[i, j]))
                {
                    int regionNum = regions[i, j];
                    if (!isolatedRegions.ContainsKey(regionNum))
                    {
                        isolatedRegions[regionNum] = new List<(int row, int col)>();
                    }
                    isolatedRegions[regionNum].Add((i, j));
                }
            }
        }

        // For each isolated region, find and create the shortest path to connect it
        foreach (var region in isolatedRegions.Values)
        {
            ConnectRegionToMain(region, fixedGrid);
        }

        return fixedGrid;
    }

    private void ConnectRegionToMain(List<(int row, int col)> isolatedRegion, int[,] fixedGrid)
    {
        HashSet<(int row, int col)> visited = new HashSet<(int row, int col)>();
        Queue<(int row, int col, List<(int row, int col)> path)> queue = new Queue<(int row, int col, List<(int row, int col)> path)>();

        // Start BFS from all cells in the isolated region
        foreach (var start in isolatedRegion)
        {
            queue.Enqueue((start.row, start.col, new List<(int row, int col)>()));
            visited.Add((start.row, start.col));
        }

        // BFS to find shortest path to any reachable region
        while (queue.Count > 0)
        {
            var (currentRow, currentCol, currentPath) = queue.Dequeue();

            // Check if we've reached a cell that's part of the main reachable region
            if (fixedGrid[currentRow, currentCol] != 0 &&
                reachableRegions.Contains(regions[currentRow, currentCol]))
            {
                // Convert zeros in the path to twos
                foreach (var (pathRow, pathCol) in currentPath)
                {
                    if (fixedGrid[pathRow, pathCol] == 0)
                    {
                        fixedGrid[pathRow, pathCol] = 2;
                    }
                }
                return;
            }

            // Check all four directions
            int[] dx = { -1, 0, 1, 0 };
            int[] dy = { 0, 1, 0, -1 };

            for (int i = 0; i < 4; i++)
            {
                int newRow = currentRow + dx[i];
                int newCol = currentCol + dy[i];

                if (IsValidPosition(newRow, newCol) && !visited.Contains((newRow, newCol)))
                {
                    var newPath = new List<(int row, int col)>(currentPath);
                    if (fixedGrid[newRow, newCol] == 0)
                    {
                        newPath.Add((newRow, newCol));
                    }
                    queue.Enqueue((newRow, newCol, newPath));
                    visited.Add((newRow, newCol));
                }
            }
        }
    }

    private void LabelRegion(int row, int col, int regionNumber)
    {
        if (!IsValidPosition(row, col) || grid[row, col] == 0 || regions[row, col] != 0)
            return;

        regions[row, col] = regionNumber;

        // Check all four directions
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        for (int i = 0; i < 4; i++)
        {
            int newRow = row + dx[i];
            int newCol = col + dy[i];
            LabelRegion(newRow, newCol, regionNumber);
        }
    }

    private bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < rows && col >= 0 && col < cols;
    }
}

