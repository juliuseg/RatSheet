using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    private Grid<int> integrationField; // The integration field with distances
    private Grid<Vector2> flowField;    // The resulting flow field with direction vectors

    public FlowField(Grid<int> integrationField)
    {
        this.integrationField = integrationField;
        flowField = new Grid<Vector2>(integrationField.Rows, integrationField.Cols, integrationField.CellSize, integrationField.GetWorldPosition(0, 0));
        GenerateFlowField();
    }

    private void GenerateFlowField()
    {
        for (int y = 0; y < flowField.Rows; y++)
        {
            for (int x = 0; x < flowField.Cols; x++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);
                Vector2 direction = CalculateFlowDirection(currentPos);
                flowField.SetGridValue(y, x, direction);
            }
        }
    }

    public Vector2 CalculateFlowDirection(Vector2Int position)
    {
        
        List<Vector2Int> neighbors = GetNeighbors(position);
        Vector2Int lowestNeighbor = position;
        int lowestValue = integrationField.GetGridValue(position.y, position.x);

        bool hasObstacle = false;
        Vector2 summedDirection = Vector2.zero;

        // Separate lists for cardinal and diagonal neighbors
        List<Vector2Int> cardinalNeighbors = new List<Vector2Int>();
        List<Vector2Int> diagonalNeighbors = new List<Vector2Int>();

        // Split neighbors into cardinal and diagonal
        foreach (Vector2Int neighbor in neighbors)
        {
            if (Mathf.Abs(neighbor.x - position.x) + Mathf.Abs(neighbor.y - position.y) == 1)
            {
                cardinalNeighbors.Add(neighbor); // Up, down, left, right
            }
            else
            {
                diagonalNeighbors.Add(neighbor); // Diagonals
            }
        }

        // Check for obstacles among all neighbors
        foreach (Vector2Int neighbor in neighbors)
        {
            int neighborValue = integrationField.GetGridValue(neighbor.y, neighbor.x);
            
            if (neighborValue == int.MaxValue)
            {
                hasObstacle = true;
                break;
            }
        }

        // If there's an obstacle, only consider cardinal neighbors for the minimum direction
        if (hasObstacle)
        {
            foreach (Vector2Int neighbor in neighbors)
            {
                int neighborValue = integrationField.GetGridValue(neighbor.y, neighbor.x);
                if (neighborValue < lowestValue)
                {
                    lowestValue = neighborValue;
                    lowestNeighbor = neighbor;
                }
            }
            return new Vector2(lowestNeighbor.x - position.x, lowestNeighbor.y - position.y).normalized;
        }

        // If no obstacles, average all neighbors (both cardinal and diagonal)
        foreach (Vector2Int neighbor in neighbors)
        {
            int currentValue = integrationField.GetGridValue(position.y, position.x);
            int neighborValue = integrationField.GetGridValue(neighbor.y, neighbor.x);
            
            Vector2 direction = new Vector2(neighbor.x - position.x, neighbor.y - position.y);
            float weight = currentValue - neighborValue;
            summedDirection += direction * weight;
        }

        return summedDirection.normalized;
    }


    private List<Vector2Int> GetNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = 
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1) // Diagonals
        };

        foreach (var direction in directions)
        {
            Vector2Int neighborPos = position + direction;
            if (neighborPos.x >= 0 && neighborPos.x < integrationField.Cols &&
                neighborPos.y >= 0 && neighborPos.y < integrationField.Rows)
            {
                neighbors.Add(neighborPos);
            }
        }
        return neighbors;
    }

    public Grid<Vector2> GetFlowField() => flowField;
    
}