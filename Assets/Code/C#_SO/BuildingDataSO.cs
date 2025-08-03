using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData_", menuName = "BuildingDataSO")]
public class BuildingDataSO : ScriptableObject
{
    [Range(0, 100)] public int completeness;
    [Range(0, 100)] public int respawnRate;
    [Range(0, 1)] public float maxHeight;
    [Range(0, 1)] public float minHeight;
    [Range(0,1000)]public int maxDistance;
    //public LootsTableSO lootTable;
    public Vector2Int size;
    public int[] backGroundData;
    public int[] ornamentData;
    public int[] groundData;
    public int II2I(int x, int y)
    {
        return x * size.y + y;
    }
}
