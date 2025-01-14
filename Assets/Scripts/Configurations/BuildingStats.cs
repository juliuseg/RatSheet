using UnityEngine;

[CreateAssetMenu(fileName = "New Building Stats", menuName = "Building Stats")]
public class BuildingStats : SelectableStats {
    [Range(0f, 5)] public float SpawnRadius = 2f;

    [Header("Spawn Objects")]
    public SpawnObject[] spawnObjects;
}