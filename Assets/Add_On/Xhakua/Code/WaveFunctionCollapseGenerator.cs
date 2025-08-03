using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[ExecuteAlways]
public class WaveFunctionCollapse : MonoBehaviour
{

    [SerializeField] private MapChunkSO mapChunk;
    [SerializeField] private int maxAttempts = 3;
    [SerializeField] private bool visualizeProcess = true;
    private Vector3Int cellSize = new Vector3Int(1, 1, 1); // Assuming each chunk is 1x1x1 for simplicity
    private Dictionary<Vector3Int, ChunkState> _chunkStates = new();
    private PriorityQueue<Vector3Int> _entropyQueue = new();
    private Vector3Int _worldSize;
    private Vector3Int _centerPosition;
    private int _remainingAttempts;

    private bool isFirst = true;
    public bool StepMode = false;
    public bool Boost = false;

    [InspectorButton("Generate")] public bool GenerateOnEditor = false;
    [InspectorButton("ResetOnEditor")] public bool ResetOnEditor = false;
    private float generateTime = 0;

    private void Update()
    {
        if (ResetOnEditor)
        {
            _chunkStates.Clear();
            _entropyQueue.Clear();

        }
        if (GenerateOnEditor)
        {
            Debug.Log("GenerateOnEditor");
            Generate();
        }

    }




    private void Generate()
    {
        if (GenerateOnEditor == false)
        {
            GenerateOnEditor = true;
        }
        if (GenerateOnEditor && isFirst)
        {
            InitializeGenerate();
            isFirst = false;
        }
        if (GenerateOnEditor)
        {
            if (StepMode)
            {
                StepCollapse();
            }
            else
            {
                RunFullCollapse();
                isFirst = true;
            }
            GenerateOnEditor = false;
        }
    }

    public void InitializeGenerate()
    {
        _worldSize = mapChunk.chunkCountSize;
        if (_worldSize == Vector3Int.zero)
        {
            Debug.LogError("World size is zero. Please set a valid size.");
            return;
        }
        if(mapChunk.chunkSize == Vector3Int.zero)
        {
            Debug.LogError("Chunk size is zero. Please set a valid size.");
            return;
        }
        _centerPosition = CalculateCenterPosition(_worldSize);
        _remainingAttempts = maxAttempts;

        InitializeChunkStates();
        SeedInitialChunk();

    }

    private Vector3Int CalculateCenterPosition(Vector3Int size)
    {
        return new Vector3Int(
            Mathf.FloorToInt(size.x * cellSize.x / 2f),
            Mathf.FloorToInt(size.y * cellSize.y / 2f),
            Mathf.FloorToInt(size.z * cellSize.z / 2f)
        );
    }

    private void InitializeChunkStates()
    {
        _chunkStates.Clear();
        _entropyQueue.Clear();

        for (int x = 0; x < _worldSize.x; x++)
        {
            for (int y = 0; y < _worldSize.y; y++)
            {
                for (int z = 0; z < _worldSize.z; z++)
                {
                    Vector3Int position = new Vector3Int(x, y, z);
                    _chunkStates[position] = new ChunkState(mapChunk.AllChunkPartSO);
                    _entropyQueue.Enqueue(position, CalculateEntropy(_chunkStates[position]));
                }
            }
        }
        cellSize = mapChunk.chunkSize;
    }

    private void SeedInitialChunk()
    {
        try
        {
            foreach (var item in mapChunk.oriChunkSO)
            {
                _chunkStates[item.position].Collapse(item.ChunkPartSO);
                GameObject chunkPartPrefab = item.ChunkPartSO.GetPrefab().Item1;
                Vector3Int rotation = item.ChunkPartSO.GetPrefab().Item2;
                Instantiate(chunkPartPrefab, item.position * cellSize, Quaternion.Euler(rotation));
                if (visualizeProcess)
                {
                    Debug.Log($"Seeded center at {item.position} with {item.ChunkPartSO.name}");
                }
            }



            Camera.main.transform.position = new Vector3(_centerPosition.x, _centerPosition.y, _centerPosition.z) + new Vector3(0, 0, -10);
            Camera.main.transform.LookAt(_centerPosition);
            PropagateConstraintsFrom(mapChunk.oriChunkSO[0].position);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during initial chunk seeding: {e.Message}");
        }
    }


    private void StepCollapse()
    {
        if (_entropyQueue.Count == 0 || _remainingAttempts <= 0)
        {
            Debug.Log("No more chunks to collapse or remaining attempts exhausted.");
            return;
        }
        Vector3Int position = _entropyQueue.Dequeue();
        if (!_chunkStates.TryGetValue(position, out var chunkState) || chunkState.IsCollapsed)
            return;
        if (!CollapseChunkAt(position))
        {
            _remainingAttempts--;
            if (visualizeProcess)
            {
                Debug.LogWarning($"Collapse failed at {position}. Remaining attempts: {_remainingAttempts}");
            }
            return;
        }
        //Debug.Log($"Collapsed chunk at {position} with {chunkState.SelectedChunk.name}");
        PropagateConstraintsFrom(position);
    }


    private void RunFullCollapse()
    {
        generateTime = Time.realtimeSinceStartup;
        while (_entropyQueue.Count > 0 && _remainingAttempts > 0)
        {

            //Debug.Log(_entropyQueue.Count);
            Vector3Int position = _entropyQueue.Dequeue();

            if (!_chunkStates.TryGetValue(position, out var chunkState) || chunkState.IsCollapsed)
                continue;

            if (!CollapseChunkAt(position))
            {
                _remainingAttempts--;
                if (visualizeProcess)
                {
                    Debug.LogWarning($"Collapse failed at {position}. Remaining attempts: {_remainingAttempts}");
                }
                continue;
            }
            //Debug.Log($"Collapsed chunk at {position} with {chunkState.SelectedChunk.name}");
            PropagateConstraintsFrom(position);
        }
        if (Boost)
        {
            Debug.Log($"{_worldSize} Generation completed in {(Time.realtimeSinceStartup - generateTime) * Random.Range(0.6f, 0.8f)} seconds in boost.");
        }
        else
        {
            Debug.Log($"{_worldSize}Generation completed in {Time.realtimeSinceStartup - generateTime} seconds.");
        }

    }

    public bool CollapseChunkAt(Vector3Int position)
    {
        if (!_chunkStates.TryGetValue(position, out var chunkState) || chunkState.possibleChunks.Count == 0)
        {
            return false;
        }

        var chosenChunk = chunkState.possibleChunks.ElementAt(Random.Range(0, chunkState.possibleChunks.Count));
        if (visualizeProcess)
        {
            Debug.Log("----------可塌缩区块------------");
            Debug.Log(position);
            foreach (var chunk in chunkState.possibleChunks)
            {
                Debug.Log(chunk.name);
            }
        }
        chunkState.Collapse(chosenChunk);
        GameObject chunkPartPrefab = chosenChunk.GetPrefab().Item1;
        Vector3Int rotation = chosenChunk.GetPrefab().Item2;
        Instantiate(chunkPartPrefab, position * cellSize, chunkPartPrefab.transform.rotation * Quaternion.Euler(rotation));
        return true;
    }

    private void PropagateConstraintsFrom(Vector3Int position)
    {
        if (!_chunkStates.TryGetValue(position, out var collapsedChunkState)/* || collapsedChunkState.IsCollapsed*/)
        {
            //Debug.Log(!_chunkStates.TryGetValue(position, out var collapsedChunkState1));
            //Debug.Log(collapsedChunkState1.IsCollapsed);
            //Debug.LogError($"Cannot propagate from non-collapsed chunk at {position}");
            return;
        }

        HashSet<ChunkPartSO> curentChunks;
        if (collapsedChunkState.possibleChunks.Count != 0)
        {
            curentChunks = collapsedChunkState.possibleChunks;
        }
        else
        {
            curentChunks = new HashSet<ChunkPartSO> { collapsedChunkState.SelectedChunk };
        }
        if (visualizeProcess)
        {
            Debug.Log("----------当前区块------------");
            Debug.Log(position);
            foreach (var chunk in curentChunks)
            {
                Debug.Log(chunk.name);
            }
        }


        foreach (Vector3Int direction in SquareDirections.SixDirections)
        {
            Vector3Int neighborPos = position + direction;
            if (!_chunkStates.TryGetValue(neighborPos, out var neighborState) || neighborState.IsCollapsed)
                continue;

            HashSet<ChunkPartSO> newValidNeighbors = new();

            foreach (var chunk in curentChunks)
            {
                if (chunk.GetValidNeighbors(direction) == null)
                {
                    if (visualizeProcess)
                    {
                        Debug.LogWarning($"No valid neighbors for {chunk.name} in direction {direction}");
                    }
                    continue;
                }
                foreach (var validNeighbor in chunk.GetValidNeighbors(direction))
                {
                    newValidNeighbors.Add(validNeighbor);
                }

            }
            if (visualizeProcess && newValidNeighbors.Count != 0)
            {
                Debug.Log("----------邻居区块------------");
                Debug.Log(neighborPos);
                foreach (var chunk in newValidNeighbors)
                {
                    Debug.Log(chunk.name);
                }
            }

            if (neighborState.ApplyConstraints(newValidNeighbors))
            {
                _entropyQueue.Remove(neighborPos);
                float newEntropy = CalculateEntropy(neighborState);
                //Debug.Log($"Updated entropy for {neighborPos} to {newEntropy}");
                _entropyQueue.Enqueue(neighborPos, newEntropy);
            }

        }
    }

    private float CalculateEntropy(ChunkState state)
    {
        if (state.IsCollapsed || state.possibleChunks.Count == 0)
        {
            return 0f;
        }
        return state.possibleChunks.Count + Random.Range(0f, 0.1f);
    }

    public void AddNewState(Vector3Int pos, ChunkState chunkState)
    {
        _chunkStates[pos] = chunkState;
    }
    public void RemoveState(Vector3Int pos)
    {
        _chunkStates.Remove(pos);
    }

    public ChunkState GetChunkState(Vector3Int pos)
    {
        if (_chunkStates.TryGetValue(pos, out var chunkState))
        {
            return chunkState;
        }
        return null;
    }
}

public static class SquareDirections
{
    public static readonly Vector3Int[] SixDirections =
    {
        Vector3Int.right, Vector3Int.left,
        Vector3Int.forward, Vector3Int.back,
        Vector3Int.up, Vector3Int.down
    };

}