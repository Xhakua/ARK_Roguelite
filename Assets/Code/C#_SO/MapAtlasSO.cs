using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MapAtlasSO", menuName = "MapAtlasSO")]
public class MapAtlasSO : ScriptableObject
{
    public List<CubeSO> cubes;


    public GameObject GetTile(int id)
    {
        if (id <= 0 || id > cubes.Count)
        {
            return null;
        }
        try
        {
            return cubes[id-1].cube;
        }
        catch (System.Exception)
        {
            Debug.LogError("GetTile" + id);
            Debug.LogError( id > cubes.Count);
            throw;
        }

    }
    public ItemSO GetItem(int id)
    {
        if (id < 0 || id >= cubes.Count)
        {
            return null;
        }
        return cubes[id].loot;
    }

    public CubeSO GetCube(int id)
    {
        if (id < 0 || id >= cubes.Count)
        {
            return null;
        }
        return cubes[id];
    }
    public float GetDestroyCount(int id)
    {
        if (id < 0 || id >= cubes.Count)
        {
            return 0;
        }
        return cubes[id].destroyCount;
    }
    public Color GetColor(int id)
    {
        if (id < 0 || id >= cubes.Count)
        {
            return Color.black;
        }
        return cubes[id].mapColor;
    }
    public CubeSO GetCubeSO(int id)
    {
        if (id < 0 || id >= cubes.Count)
        {
            return null;
        }
        return cubes[id];
    }
}
