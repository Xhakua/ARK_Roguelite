using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// ��¼��ǰ̮�����״̬
/// </summary>
public class ChunkState
{
    //����״̬��ʹ��HashSetȥ����Ϊ���򣬷���������
    public HashSet<ChunkPartSO> possibleChunks;
    //�Ƿ��Ѿ�̮��
    public bool IsCollapsed = false;

    //̮�����״̬
    public ChunkPartSO SelectedChunk { get; private set; }

    //��
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
    /// Ӧ��Լ�����������¿��ܵ�״̬
    /// ʹ��HashSet��IntersectWith�����󽻼�
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
