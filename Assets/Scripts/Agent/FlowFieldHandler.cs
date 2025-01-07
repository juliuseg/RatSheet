using UnityEngine;

public static class FlowFieldHandler {
    public static Vector2 CalculateFlowFieldForce(Vector3 position, MovementManager movementManager)
    {
        // Convert the agent's position to the flow field grid position
        Vector2Int gridPos = movementManager.flowFieldManager.GetGridPosition(position);
        return movementManager.flowFieldManager.GetFlowFieldValue(gridPos);

    }
}