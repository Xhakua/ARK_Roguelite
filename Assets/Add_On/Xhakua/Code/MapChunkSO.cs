using System;
using UnityEngine;
[CreateAssetMenu(fileName = "MapChunkSO", menuName = "MapChunkSO")]
public class MapChunkSO : ScriptableObject
{

    public Vector3Int chunkCountSize;
    public Vector3Int chunkSize;
    public Vector3Int size { get { return chunkCountSize * chunkSize; } }

    public ChunkPartSO[] AllChunkPartSO;
    [SerializeField] public OriChunk[] oriChunkSO;
    [Serializable]
    public struct OriChunk
    {
        public Vector3Int position;
        public ChunkPartSO ChunkPartSO;
    }
}
