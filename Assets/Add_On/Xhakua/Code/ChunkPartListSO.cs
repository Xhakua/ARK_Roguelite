using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ChunkPartListSO", menuName = "ChunkPartListSO")]
public class ChunkPartListSO : ScriptableObject
{
    [Header("正的必须要有那个数")]
    [Header("负的不能有它的相反数")]
    public bool negative = false;
    public Vector3Int direction;

    public List<ChunkPartSO> chunkPartSOs= new List<ChunkPartSO>();


}
