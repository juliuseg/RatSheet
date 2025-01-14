using UnityEngine;

[CreateAssetMenu(fileName = "New Spawn Object", menuName = "Spawn Object")]
public class SpawnObject : ScriptableObject
{
    public GameObject objectToSpawn;
    public float spawnTime;

    public string textForUI;

    
}