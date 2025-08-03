using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PerlinMapSO", menuName = "PerlinMapSO")]
public class PerlinMapSO : ScriptableObject
{
    public Vector3Int worldSize;
    public CubeSO wall;
    public CubeSO floor;

    public Mine[] mines;
    public Enemy[] enemies;
    [Serializable]
    public struct Mine
    {
        public CubeSO mine;
        public float frequency;
        public float abundance;
        public int maxHeight;
        public int minHeight;
        public int seed;
    }
    [Serializable]
    public struct Enemy
    {
        public GameObject enemy;
        public float respawnRate;
        public bool isDay;
        public bool isUp;
    }
}
