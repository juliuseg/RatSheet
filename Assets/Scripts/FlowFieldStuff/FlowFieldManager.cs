// using UnityEngine;

// public class FlowFieldManager 
// {
//     public int rows = 40;
//     public int cols = 40;
//     public float cellSize = 0.5f;
//     public LayerMask terrainLayer; 
//     public Vector3 targetPoint; 

//     private Grid<int> weightField;
//     private IntegrationField integrationField;
//     private FlowField flowField;

//     public FlowFieldManager(int _rows, int _cols, float _cellSize, LayerMask _terrainLayer, Vector3 _targetPoint) // Potentionaly add weight grid so we don't have to recalculate it every time
//     {
//         rows = _rows;
//         cols = _cols;
//         cellSize = _cellSize;
//         terrainLayer = _terrainLayer;
//         targetPoint = _targetPoint;

//         Vector3 gridCenterOffset = new Vector3(Mathf.Floor((cols * cellSize) / 2), Mathf.Floor((rows * cellSize) / 2), 0);        
//         weightField = new Grid<int>(rows, cols, cellSize, -gridCenterOffset);

//         UpdateWeightGrid();


//     }

//     public bool CreateGridFromMousePos(Vector3 mousePos) {
        
//         Vector2Int mousepos = weightField.GetGridPosition(mousePos);
//         Vector3 mousePosBack = weightField.GetWorldPosition(mousepos.y, mousepos.x);
//         targetPoint = new Vector3(mousePosBack.x, mousePosBack.y, 0);

//         integrationField = new IntegrationField(weightField, targetPoint);
//         flowField = new FlowField(integrationField.DistanceField);

//         if (weightField.GetGridValue(mousepos.y, mousepos.x) == int.MaxValue)
//         {
//             Debug.Log("Target is on impassable terrain");
//             return false;
//         }
//         return true;
        
        
//     }

//     private void UpdateWeightGrid(){
//         // Initialize weight map by checking terrain at each cell
//         for (int x = 0; x < rows; x++)
//         {
//             for (int y = 0; y < cols; y++)
//             {
//                 Vector3 cellPosition = weightField.GetWorldPosition(x, y);
//                 weightField.SetGridValue(x, y, DetermineWeight(cellPosition));
//             }
//         }
//     }

//     private int DetermineWeight(Vector3 cellPosition)
//     {
//         // Use an OverlapBox to detect terrain within the cell
//         Collider2D[] colliders = Physics2D.OverlapBoxAll(cellPosition, new Vector2(cellSize, cellSize) * 0.9f, 0, terrainLayer);

//         foreach (Collider2D collider in colliders)
//         {
//             if (collider.CompareTag("Rock"))
//             {
//                 return int.MaxValue; // Impassable terrain
//             }
//             else if (collider.CompareTag("Grass"))
//             {
//                 return 2; // Passable terrain with normal weight
//             }
//         }

//         return 1; // Default to passable if no collider is detected
//     }

//     public Vector2 GetFlowFieldValue(Vector2Int position) {
        
//         return flowField.CalculateFlowDirection(position);

//     }

//     public Vector2Int GetGridPosition(Vector3 position) {
//         return weightField.GetGridPosition(position);
//     }

//     public Grid<Vector2> GetFlowField() => flowField.GetFlowField();
//     public Grid<int> WeightField => weightField;
//     public Grid<int> GetIntegrationField() => integrationField.DistanceField;
// }

using UnityEngine;
using System.Collections.Generic;

public class FlowFieldManager 
{
    public int rows = 40;
    public int cols = 40;
    public float cellSize = 0.5f;
    public LayerMask terrainLayer; 
    public List<Vector3> targetPoint; 

    private Grid<int> weightField;
    private IntegrationField integrationField;
    private FlowField flowField;

    public FlowFieldManager(int _rows, int _cols, float _cellSize, LayerMask _terrainLayer) // Potentionaly add weight grid so we don't have to recalculate it every time
    {
        rows = _rows;
        cols = _cols;
        cellSize = _cellSize;
        terrainLayer = _terrainLayer;

        Vector3 gridCenterOffset = new Vector3(Mathf.Floor((cols * cellSize) / 2), Mathf.Floor((rows * cellSize) / 2), 0);        
        weightField = new Grid<int>(rows, cols, cellSize, -gridCenterOffset);

        UpdateWeightGrid();


    }

    public bool CreateGridFromMousePos(Vector3 mousePos) {
        
        Vector2Int mousepos = weightField.GetGridPosition(mousePos);
        targetPoint = new List<Vector3>();

        // for (int i = -1; i <= 1; i++)
        // {
        //     for (int j = -1; j <= 1; j++)
        //     {
        //         targetPoint.Add(weightField.GetWorldPosition(mousepos.y + i, mousepos.x + j));
        //     }
        // }

        targetPoint.Add(weightField.GetWorldPosition(mousepos.y, mousepos.x));


        

        integrationField = new IntegrationField(weightField, targetPoint);
        flowField = new FlowField(integrationField.DistanceField);

        if (weightField.GetGridValue(mousepos.y, mousepos.x) == int.MaxValue)
        {
            Debug.Log("Target is on impassable terrain");
            return false;
        }
        return true;
        
        
    }


    private void UpdateWeightGrid(){
        // Initialize weight map by checking terrain at each cell
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                Vector3 cellPosition = weightField.GetWorldPosition(x, y);
                weightField.SetGridValue(x, y, DetermineWeight(cellPosition));
            }
        }
    }

    private int DetermineWeight(Vector3 cellPosition)
    {
        // Use an OverlapBox to detect terrain within the cell
        Collider2D[] colliders = Physics2D.OverlapBoxAll(cellPosition, new Vector2(cellSize, cellSize) * 0.9f, 0, terrainLayer);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Rock"))
            {
                return int.MaxValue; // Impassable terrain
            }
            else if (collider.CompareTag("Grass"))
            {
                return 2; // Passable terrain with normal weight
            }
        }

        return 1; // Default to passable if no collider is detected
    }

    public Vector2 GetFlowFieldValue(Vector2Int position) {
        
        return flowField.CalculateFlowDirection(position);

    }

    public Vector2Int GetGridPosition(Vector3 position) {
        return weightField.GetGridPosition(position);
    }

    public Grid<Vector2> GetFlowField() => flowField.GetFlowField();
    public Grid<int> WeightField => weightField;
    public Grid<int> GetIntegrationField() => integrationField.DistanceField;
}