using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// 记录当前坍缩点的状态
/// </summary>
public class ChunkState
{
    //可能状态，使用HashSet去重且为无序，方便后续随机
    public HashSet<ChunkPartSO> possibleChunks;
    //是否已经坍缩
    public bool IsCollapsed = false;

    //坍缩后的状态
    public ChunkPartSO SelectedChunk { get; private set; }

    //熵
    public float Entropy => possibleChunks.Count;

    public ChunkState(ChunkPartSO[] chunks)
    {
        possibleChunks = new HashSet<ChunkPartSO>(chunks);
    }

    public ChunkState(ChunkPartSO chunk)
    {
        possibleChunks = new HashSet<ChunkPartSO> { chunk };
    }



    public bool Collapse()
    {
        UnityEngine.Debug.Log(possibleChunks.Count);
        if (possibleChunks.Count == 0) return false;
        var enumerator = possibleChunks.GetEnumerator();
        enumerator.MoveNext();
        SelectedChunk = enumerator.Current;
        possibleChunks.Clear();

        return true;
    }

    public void Collapse(ChunkPartSO chunk)
    {
        SelectedChunk = chunk;
        possibleChunks.Clear();
        IsCollapsed = true;
    }

    /// <summary>
    /// 应用约束条件，更新可能的状态
    /// 使用HashSet的IntersectWith方法求交集
    /// </summary>
    /// <param name="allowedChunks"></param>
    /// <returns></returns>
    public bool ApplyConstraints(HashSet<ChunkPartSO> allowedChunks)
    {
        int previousCount = possibleChunks.Count;
        possibleChunks.IntersectWith(allowedChunks);
        return possibleChunks.Count != previousCount;
    }

}
