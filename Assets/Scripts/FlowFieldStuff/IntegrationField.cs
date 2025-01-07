using System.Collections.Generic;
using UnityEngine;

public class IntegrationField
{
    private Grid<int> weightField; 
    private Grid<int> integrationField; 
    private List<Vector2Int> targetPoint;

    public IntegrationField(Grid<int> weightField, List<Vector3> targetPositions)
    {
        this.weightField = weightField;
        targetPoint = new List<Vector2Int>();
        foreach (Vector3 targetPosition in targetPositions)
        {
            targetPoint.Add(weightField.GetGridPosition(targetPosition)); // Convert targetPosition to grid coordinates
        }
        integrationField = new Grid<int>(weightField.Rows, weightField.Cols, weightField.CellSize, weightField.GetWorldPosition(0, 0));

        ComputeIntegrationField();
        //InitializeDistances();
    }
    private void ComputeIntegrationField()
    {
        float startTime = Time.realtimeSinceStartup;

        int width = integrationField.Cols;
        int height = integrationField.Rows;

        // Initialize all cells to a large value
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                integrationField.GridArray[x, y] = int.MaxValue;
            }
        }

        // Priority queue for BFS
        MinHeap<(Vector2Int, int)> queue = new MinHeap<(Vector2Int, int)>(
            (a, b) => a.Item2.CompareTo(b.Item2)); // Use Item2 to compare the cost (int)

        // Add the end point to the queue
        foreach (Vector2Int target in targetPoint)
        {
            queue.Enqueue((target, 0));
            integrationField.GridArray[target.x, target.y] = 0;
        }

        // Directions for neighbor cells (cardinal and diagonal)
        Vector2Int[] cardinalDirections = {
            new Vector2Int(0, 1), new Vector2Int(1, 0),
            new Vector2Int(0, -1), new Vector2Int(-1, 0)
        };

        Vector2Int[] diagonalDirections = {
            new Vector2Int(-1, -1), new Vector2Int(-1, 1),
            new Vector2Int(1, -1), new Vector2Int(1, 1)
        };

        int iterations = 0;

        // Process the queue
        while (queue.Count > 0)
        {
            iterations++;

            var (current, cost) = queue.Dequeue();

            // Process cardinal neighbors (cost = 1)
            foreach (var dir in cardinalDirections)
            {
                Vector2Int neighbor = current + dir;
                if (neighbor.x >= 0 && neighbor.x < width &&
                    neighbor.y >= 0 && neighbor.y < height &&
                    weightField.GridArray[neighbor.x, neighbor.y] != int.MaxValue)
                {
                    
                    int newCost = cost + 1;
                    if (newCost < integrationField.GridArray[neighbor.x, neighbor.y])
                    {
                        integrationField.GridArray[neighbor.x, neighbor.y] = newCost;
                        queue.Enqueue((neighbor, newCost));
                    }
                }
            }

            // Process diagonal neighbors (cost = 2)
            foreach (var dir in diagonalDirections)
            {
                Vector2Int neighbor = current + dir;

                if (neighbor.x >= 0 && neighbor.x < width &&
                    neighbor.y >= 0 && neighbor.y < height &&
                    weightField.GridArray[neighbor.x, neighbor.y] != int.MaxValue)
                {
                    int newCost = cost + 2;
                    if (newCost < integrationField.GridArray[neighbor.x, neighbor.y])
                    {
                        integrationField.GridArray[neighbor.x, neighbor.y] = newCost;
                        queue.Enqueue((neighbor, newCost));
                    }
                }
            }
        }

        float endTime = Time.realtimeSinceStartup;

        //Debug.Log("Integration field computed in " + (endTime - startTime) + " seconds with " + iterations + " iterations");

    }



    public Grid<int> DistanceField => integrationField;
}