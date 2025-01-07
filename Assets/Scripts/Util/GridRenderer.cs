using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    public enum RenderMode { WeightField, IntegrationField, FlowField }
    public RenderMode renderMode;

    public FlowFieldManager flowFieldManager;
    public Color gridColor = Color.white;
    public Color vectorColor = Color.blue;
    public Color weightColor = Color.green;
    public Color integrationColor = Color.yellow;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            if (flowFieldManager == null) return;

            int rows = flowFieldManager.rows;
            int cols = flowFieldManager.cols;
            float cellSize = flowFieldManager.cellSize;
            
            // Calculate the center offset
            Vector3 gridCenterOffset = new Vector3(Mathf.Floor((cols * cellSize) / 2) - cellSize/2, Mathf.Floor((rows * cellSize) / 2) - cellSize/2, 0);
            Vector3 originPosition = -gridCenterOffset;

            Gizmos.color = gridColor;

            // Draw the empty grid
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Vector3 cellCenter = originPosition + new Vector3(x * cellSize, y * cellSize, 0);
                    Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize, cellSize, 0));
                }
            }

            return; // Exit to prevent further rendering
        }
        if (flowFieldManager == null) { 
            //print("flowfield is none!"); 
            return;
        }

        var weightField = flowFieldManager.WeightField;
        var integrationField = flowFieldManager.GetIntegrationField(); 
        var vectorField = flowFieldManager.GetFlowField(); 

        for (int x = 0; x < weightField.Rows; x++)
        {
            for (int y = 0; y < weightField.Cols; y++)
            {
                Vector3 worldPos = weightField.GetWorldPosition(x, y);
                Vector3 cellCenter = worldPos;
                
                // Draw cell outline
                Gizmos.color = gridColor;
                Gizmos.DrawWireCube(cellCenter, new Vector3(weightField.CellSize, weightField.CellSize, 0));

                // Render based on selected mode
                switch (renderMode)
                {
                    case RenderMode.WeightField:
                        DrawWeightField(weightField, x, y, cellCenter);
                        break;

                    case RenderMode.IntegrationField:
                        DrawIntegrationField(integrationField, x, y, cellCenter);
                        break;

                    case RenderMode.FlowField:
                        DrawFlowField(vectorField, x, y, cellCenter);
                        break;
                }
            }
        }
    }

    private void DrawWeightField(Grid<int> weightField, int x, int y, Vector3 cellCenter)
    {
        Gizmos.color = weightColor;
        int weightValue = weightField.GetGridValue(x, y);
        Vector3 labelPosition = cellCenter + Vector3.down * 0.1f;
        
        if (weightValue == int.MaxValue)
        {
            UnityEditor.Handles.Label(labelPosition, "X"); // Representing obstacles
        }
        else
        {
            UnityEditor.Handles.Label(labelPosition, weightValue.ToString());
        }
    }

    private void DrawIntegrationField(Grid<int> integrationField, int x, int y, Vector3 cellCenter)
    {
        Gizmos.color = integrationColor;
        int distanceValue = integrationField.GetGridValue(x, y);
        Vector3 labelPosition = cellCenter + Vector3.down * 0.1f;
        
        if (distanceValue == int.MaxValue)
        {
            UnityEditor.Handles.Label(labelPosition, "∞"); // Representing unreachable cells
        }
        else
        {
            UnityEditor.Handles.Label(labelPosition, distanceValue.ToString());
        }
    }

    private void DrawFlowField(Grid<Vector2> vectorField, int y, int x, Vector3 cellCenter)
    {
        Gizmos.color = vectorColor;
        Vector2 vectorValue = vectorField.GetGridValue(y, x);

        // Calculate the start and end of the main direction arrow
        Vector3 vectorStart = cellCenter;
        Vector3 vectorEnd = cellCenter + (Vector3)vectorValue.normalized * vectorField.CellSize * 0.4f;
        Gizmos.DrawLine(vectorStart, vectorEnd);

        // Arrowhead size and angle
        float arrowheadLength = 0.15f * vectorField.CellSize;
        float arrowheadAngle = 20f;

        // Calculate the two points for the arrowhead
        Vector3 direction = (vectorEnd - vectorStart).normalized;
        Vector3 rightArrowhead = Quaternion.Euler(0, 0, arrowheadAngle) * -direction * arrowheadLength;
        Vector3 leftArrowhead = Quaternion.Euler(0, 0, -arrowheadAngle) * -direction * arrowheadLength;

        // Draw the arrowhead lines
        Gizmos.DrawLine(vectorEnd, vectorEnd + rightArrowhead);
        Gizmos.DrawLine(vectorEnd, vectorEnd + leftArrowhead);
    }
}