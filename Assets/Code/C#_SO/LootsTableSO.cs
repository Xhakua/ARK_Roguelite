using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LootsTableSO", menuName = "LootsTableSO")]
public class LootsTableSO : ScriptableObject
{
    [Serializable]
    public struct Loots
    {
        public ItemSO itemSO;
        [Range(0, 100)] public int dropRate;
        public int amount;
    }

    public List<Loots> lootsTable;
}
