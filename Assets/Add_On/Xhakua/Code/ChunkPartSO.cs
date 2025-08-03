using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChunkPartSO", menuName = "ChunkPartSO")]
public class ChunkPartSO : ScriptableObject
{
    [Serializable]
    public struct ChunkPartData
    {
        public bool isParent;
        public bool rotaryShaft_X;
        public bool rotaryShaft_Y;
        public bool rotaryShaft_Z;
        public Vector3 rotaryShaft { get { return new Vector3(rotaryShaft_X ? 1 : 0, rotaryShaft_Y ? 1 : 0, rotaryShaft_Z ? 1 : 0); } }
        public List<Vector3Int> directions;

    }
    [SerializeField] private bool randomDirection = false;
    [SerializeField] private Vector3Int directionUnit = Vector3Int.zero;
    [SerializeField] private bool randomGameObject = false;
    [Header("对应数值默认为负数")]
    [SerializeField] private int _sizeX = 1;
    [SerializeField] private int _sizeY = 1;
    [SerializeField] private int _sizeZ = 1;
    [SerializeField] private List<GameObject> _chunkPart;
    public Vector3Int needDir = Vector3Int.zero;
    public ChunkPartData chunkPartData = new ChunkPartData();

    public Vector3Int Size => new Vector3Int(_sizeX, _sizeY, _sizeZ);
    [Header("1 Up")]
    [SerializeField] private List<ChunkPartListSO> neighbor_Y_Positive = new List<ChunkPartListSO>();
    [Header("2 Right")]
    [SerializeField] private List<ChunkPartListSO> neighbor_X_Positive = new List<ChunkPartListSO>();
    [Header("3 Down")]
    [SerializeField] private List<ChunkPartListSO> neighbor_Y_Negative = new List<ChunkPartListSO>();
    [Header("4 Left")]
    [SerializeField] private List<ChunkPartListSO> neighbor_X_Negative = new List<ChunkPartListSO>();
    [Header("5 Forword")]
    [SerializeField] private List<ChunkPartListSO> neighbor_Z_Positive = new List<ChunkPartListSO>();
    [Header("6 Back")]
    [SerializeField] private List<ChunkPartListSO> neighbor_Z_Negative = new List<ChunkPartListSO>();

    private Dictionary<Vector3Int, List<ChunkPartListSO>> _neighborMap;
    private bool _neighborMapInitialized;

    [SerializeField] private int[] _serializedData;
    [System.NonSerialized] private int[,,] _runtimeData;

    public (GameObject, Vector3Int) GetPrefab()
    {
        if (_chunkPart == null)
        {
            Debug.LogError("ChunkPart prefab is not assigned.");
            return (null, Vector3Int.zero);
        }
        if (randomDirection)
        {
            needDir = new Vector3Int(
              directionUnit.x * UnityEngine.Random.Range(-5, 5),
              directionUnit.y * UnityEngine.Random.Range(-5, 5),
              directionUnit.z * UnityEngine.Random.Range(-5, 5));
        }
        GameObject randomPrefab;
        if (randomGameObject)
        {
            randomPrefab = _chunkPart[UnityEngine.Random.Range(0, _chunkPart.Count)];
        }
        else
        {
            randomPrefab = _chunkPart[0];
        }
        return (randomPrefab, needDir);
    }


    public void SetPrefab(GameObject prefab)
    {
        if (_chunkPart == null)
        {
            _chunkPart = new List<GameObject>();
        }
        if (prefab != null && !_chunkPart.Contains(prefab))
        {
            _chunkPart.Add(prefab);
        }
    }
    public int[,,] Data
    {
        get
        {
            ValidateDataStructure();
            return _runtimeData;
        }
    }

    private void InitializeNeighborMap()
    {
        _neighborMap = new Dictionary<Vector3Int, List<ChunkPartListSO>>
        {
            { Vector3Int.right,   neighbor_X_Positive },
            { Vector3Int.left,    neighbor_X_Negative },
            { Vector3Int.up,       neighbor_Y_Positive },
            { Vector3Int.down,     neighbor_Y_Negative },
            { Vector3Int.forward,  neighbor_Z_Positive },
            { Vector3Int.back,     neighbor_Z_Negative }
        };
        _neighborMapInitialized = true;
    }

    public bool IsValidNeighbor(ChunkPartSO chunk, Vector3Int direction)
    {
        if (!IsValidDirection(direction)) return false;

        if (!_neighborMapInitialized || _neighborMap == null) InitializeNeighborMap();

        if (_neighborMap.TryGetValue(direction, out var neighbors) &&
            neighbors != null &&
            neighbors[0].chunkPartSOs.Count > 0)
        {
            foreach (var neighbor in neighbors[0].chunkPartSOs)
            {
                if (neighbor == chunk) return true;
            }
        }
        return false;
    }

    public HashSet<ChunkPartSO> GetValidNeighbors(Vector3Int direction)
    {
        if (!IsValidDirection(direction)) return null;
        if (!_neighborMapInitialized || _neighborMap == null) InitializeNeighborMap();
        if (_neighborMap.TryGetValue(direction, out var neighbors) &&
            neighbors != null &&
            neighbors[0].chunkPartSOs.Count > 0)
        {
            return new HashSet<ChunkPartSO>(neighbors[0].chunkPartSOs);
        }
        return null;
    }
    public bool IsValidNeighbor(HashSet<ChunkPartSO> chunks, Vector3Int direction)
    {
        if (!IsValidDirection(direction)) return false;
        if (!_neighborMapInitialized || _neighborMap == null) InitializeNeighborMap();
        foreach (var chunk in chunks)
        {
            if (chunk._neighborMap.TryGetValue(direction, out var neighbors) &&
                neighbors != null &&
                neighbors[0].chunkPartSOs.Count > 0)
            {
                foreach (var neighbor in neighbors[0].chunkPartSOs)
                {
                    if (neighbor == chunk) return true;
                }
            }
        }
        return false;
    }



    private bool IsValidDirection(Vector3Int direction)
    {
        int components = 0;
        components += Mathf.Abs(direction.x);
        components += Mathf.Abs(direction.y);
        components += Mathf.Abs(direction.z);
        return components == 1;
    }

    public void Clear(Vector3Int newSize)
    {
        _sizeX = newSize.x;
        _sizeY = newSize.y;
        _sizeZ = newSize.z;
        InitializeArrays();
    }

    public void Save()
    {
        _serializedData = new int[_sizeX * _sizeY * _sizeZ];
        int index = 0;

        for (int y = 0; y < _sizeY; y++)
        {
            for (int z = 0; z < _sizeZ; z++)
            {
                for (int x = 0; x < _sizeX; x++)
                {
                    _serializedData[index++] = _runtimeData[x, y, z];
                }
            }
        }
    }

    private void InitializeArrays()
    {
        _runtimeData = new int[_sizeX, _sizeY, _sizeZ];
        _serializedData = new int[_sizeX * _sizeY * _sizeZ];
    }

    private void ValidateDataStructure()
    {
        bool needsRefresh = _runtimeData == null ||
            _runtimeData.GetLength(0) != _sizeX ||
            _runtimeData.GetLength(1) != _sizeY ||
            _runtimeData.GetLength(2) != _sizeZ;

        if (!needsRefresh) return;

        _runtimeData = new int[_sizeX, _sizeY, _sizeZ];

        if (_serializedData == null || _serializedData.Length != _sizeX * _sizeY * _sizeZ)
        {
            return;
        }

        int index = 0;
        for (int y = 0; y < _sizeY; y++)
        {
            for (int z = 0; z < _sizeZ; z++)
            {
                for (int x = 0; x < _sizeX; x++)
                {
                    if (index < _serializedData.Length)
                    {
                        _runtimeData[x, y, z] = _serializedData[index++];
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ValidateDataStructure();
    }

    public void SetNeighbor(ChunkPartListSO neighbor, Vector3Int direction)
    {
        if (neighbor == null) return;
        if (!_neighborMapInitialized || _neighborMap == null) InitializeNeighborMap();
        if (_neighborMap.TryGetValue(direction, out var neighbors))
        {
            if (neighbors.Count == 0)
            {
                neighbors.Add(neighbor);
            }
            else
            {
                neighbors[0] = neighbor;
            }
        }
    }
#endif
}