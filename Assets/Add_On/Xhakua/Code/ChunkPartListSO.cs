using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ChunkPartListSO", menuName = "ChunkPartListSO")]
public class ChunkPartListSO : ScriptableObject
{
    [Header("���ı���Ҫ���Ǹ���")]
    [Header("���Ĳ����������෴��")]
    public bool negative = false;
    public Vector3Int direction;

    public List<ChunkPartSO> chunkPartSOs= new List<ChunkPartSO>();


}
