using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Â·¾¶¹ÜÀíÆ÷
/// </summary>
public class PathManager : MonoBehaviour
{
    public static PathManager Instance { get; private set; }
    public MapAtlasSO allCubes;
    [SerializeField] private List<Vector2Int> targetPosList = new List<Vector2Int>();
    //public GameObject cube;
    public Vector2Int TargetPos
    {
        get
        {
            return targetPosList[0];
        }
    }
    public void AddTargetPos(Vector2Int pos)
    {
        if (pos == targetPosList[0])
        {
            return;
        }

        StopAllCoroutines();
        targetPosList[0] = pos;

        StartCoroutine(DijkstraPath(targetPosList[0], 0));
    }

    public void SetPath(List<Vector2Int> path, int minDistance)
    {
        minDistance += 100;
        for (int i = path.Count - 1; i >= 0; i--)
        {
            if (IsPosInChunk(path[i], targetPosList[0]))
            {
                continue;
            }
            DistanceField[path[i].x, path[i].y] = minDistance++;
            //Instantiate(cube, new Vector3(path[i].x, path[i].y, -1), Quaternion.identity);
        }

    }

    public void ClearTargetPos(Vector2Int pos)
    {
        if (IsPosInChunk(pos, targetPosList[0]))
        {
            return;
        }
        StopAllCoroutines();
        for (int i = 0; i < targetPosList.Count; i++)
        {
            ClearDistanceFieldChunk(MapDynamicLoadingManager.Instance.Pos2Chunk(targetPosList[i]));
        }
        //targetPosList.Clear();
    }
    private int[,] _distanceField;
    private int[,] tempDisstanceField;
    public int[,] DistanceField
    {
        get
        {
            if (_distanceField == null)
            {
                _distanceField = new int[MapManager.Instance.GetWorldSize().x, MapManager.Instance.GetWorldSize().z];
                tempDisstanceField = new int[MapManager.Instance.GetWorldSize().x, MapManager.Instance.GetWorldSize().z];
                for (int x = 0; x < MapManager.Instance.GetWorldSize().x; x++)
                {
                    for (int y = 0; y < MapManager.Instance.GetWorldSize().z; y++)
                    {
                        tempDisstanceField[x, y] = int.MaxValue;
                    }

                }
                _distanceField = tempDisstanceField.Clone() as int[,];

            }
            return _distanceField;
        }
        set
        {
            _distanceField = value;
            //tempDisstanceField = value;
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public bool IsPosInChunk(Vector2Int pos1, Vector2Int pos2)
    {
        Vector2Int chunkPos = MapDynamicLoadingManager.Instance.Pos2Chunk(pos2);
        Vector2Int dynamicMapSize = MapDynamicLoadingManager.Instance.dynamicMapSize;
        Vector2Int ldAnchor = new Vector2Int((chunkPos.x - 1) * dynamicMapSize.x, (chunkPos.y - 1) * dynamicMapSize.y);
        Vector2Int ruAnchor = new Vector2Int(ldAnchor.x + 3 * dynamicMapSize.x - 1, ldAnchor.y + 3 * dynamicMapSize.y - 1);

        return pos1.x >= ldAnchor.x && pos1.x <= ruAnchor.x && pos1.y >= ldAnchor.y && pos1.y <= ruAnchor.y;
    }


    public Vector2Int GetMoveDir(Vector2Int pos)
    {
        //int ld = DistanceField[pos.x - 1, pos.y - 1];
        //int rd = DistanceField[pos.x + 1, pos.y - 1];
        //int lu = DistanceField[pos.x - 1, pos.y + 1];
        //int ru = DistanceField[pos.x + 1, pos.y + 1];
        //if (true)
        //{
        //    ld = int.MaxValue;
        //    rd = int.MaxValue;
        //    lu = int.MaxValue;
        //    ru = int.MaxValue;
        //}

        int l = DistanceField[pos.x - 1, pos.y];
        int r = DistanceField[pos.x + 1, pos.y];
        int u = DistanceField[pos.x, pos.y + 1];
        int d = DistanceField[pos.x, pos.y - 1];
        int c = DistanceField[pos.x, pos.y];

        //string str = "";
        //for (int i = 0; i < 9; i++)
        //{
        //    if (i % 3 == 0)
        //    {
        //        str += "\n";
        //    }
        //    str += DistanceField[pos.x + i % 3 - 1, pos.y + i / 3 - 1] + " ";
        //}
        //Debug.Log(str);
        int min = Mathf.Min(/*ld, rd, lu, ru, */l, r, u, d, c);
        if (min == c)
        {
            //Debug.Log(str);
            if (!IsPosInChunk(pos, targetPosList[0]))
            {
                DistanceField[pos.x - 1, pos.y - 1] = int.MaxValue;
                DistanceField[pos.x + 1, pos.y - 1] = int.MaxValue;
                DistanceField[pos.x - 1, pos.y + 1] = int.MaxValue;
                DistanceField[pos.x + 1, pos.y + 1] = int.MaxValue;
                DistanceField[pos.x - 1, pos.y] = int.MaxValue;
                DistanceField[pos.x + 1, pos.y] = int.MaxValue;
                DistanceField[pos.x, pos.y + 1] = int.MaxValue;
                DistanceField[pos.x, pos.y - 1] = int.MaxValue;
                DistanceField[pos.x, pos.y] = int.MaxValue;
            }

            return new Vector2Int(0, 0);
        }
        if (min == l)
        {
            return new Vector2Int(-1, 0);
        }
        else if (min == r)
        {
            return new Vector2Int(1, 0);
        }
        else if (min == u)
        {
            return new Vector2Int(0, 1);
        }
        else /*if (min == d)*/
        {
            return new Vector2Int(0, -1);
        }


    }



    public void ClearDistanceFieldChunk(Vector2Int vector2Int)
    {
        Vector2Int dynamicMapSize = MapDynamicLoadingManager.Instance.dynamicMapSize;
        Vector2Int chunkPos = MapDynamicLoadingManager.Instance.Pos2Chunk(vector2Int);
        Vector2Int ldAnchor = new Vector2Int((chunkPos.x - 1) * dynamicMapSize.x, (chunkPos.y - 1) * dynamicMapSize.y);
        Vector2Int ruAnchor = new Vector2Int(ldAnchor.x + 3 * dynamicMapSize.x - 1, ldAnchor.y + 3 * dynamicMapSize.y - 1);

        for (int y = ldAnchor.y; y <= ruAnchor.y; y++)
        {
            for (int x = ldAnchor.x; x <= ruAnchor.x; x++)
            {
                //Debug.Log(x + " " + y);
                tempDisstanceField[x, y] = int.MaxValue;
            }
        }
        //DistanceField = tempDisstanceField.Clone() as int[,];
    }

    private void Start()
    {
        StartCoroutine(DijkstraPath(targetPosList[0], 0));
    }

    public IEnumerator DijkstraPath(Vector2Int vector2Int, int distance)
    {
        //Debug.Log("DijkstraPath");
        Vector2Int dynamicMapSize = MapDynamicLoadingManager.Instance.dynamicMapSize;
        Vector2Int chunkPos = MapDynamicLoadingManager.Instance.Pos2Chunk(vector2Int);
        Vector2Int ldAnchor = new Vector2Int((chunkPos.x - 1) * dynamicMapSize.x, (chunkPos.y - 1) * dynamicMapSize.y);
        Vector2Int ruAnchor = new Vector2Int(ldAnchor.x + 3 * dynamicMapSize.x - 1, ldAnchor.y + 3 * dynamicMapSize.y - 1);

        List<(Vector2Int position, int distance)> priorityQueue = new List<(Vector2Int, int)>();
        priorityQueue.Add((vector2Int, 0));

        int n = 0;
        Vector2Int[] dirs = new Vector2Int[]
{
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1)
};
        DistanceField[vector2Int.x, vector2Int.y] = 0;
        ClearDistanceFieldChunk(vector2Int);
        tempDisstanceField[vector2Int.x, vector2Int.y] = 0;


        while (priorityQueue.Count > 0)
        {
            var (pos, currentDist) = priorityQueue.OrderBy(p => p.distance).First();
            priorityQueue.Remove((pos, currentDist)); // Remove the selected item

            foreach (var dir in dirs)
            {
                Vector2Int newPos = pos + dir;
                Vector2Int recordPos = newPos - ldAnchor;

                n++;
                if (newPos.x < ldAnchor.x || newPos.x > ruAnchor.x || newPos.y < ldAnchor.y || newPos.y > ruAnchor.y)
                {
                    continue;
                }

                if (recordPos.x < 0 || recordPos.x >= 3 * dynamicMapSize.x || recordPos.y < 0 || recordPos.y >= 3 * dynamicMapSize.y)
                {
                    continue;
                }

                int addDistance = (MapManager.Instance.GetWorldData(MapManager.TileLayer.Wall)[newPos.x, newPos.y] != 0)
                ? allCubes.cubes[MapManager.Instance.GetWorldData(MapManager.TileLayer.Wall)[newPos.x, newPos.y]-1].destroyCount + 1
                : 1;




                int newDistance = tempDisstanceField[pos.x, pos.y] + addDistance;


                if (tempDisstanceField[newPos.x, newPos.y] <= newDistance)
                {
                    continue;
                }

                if (newPos == targetPosList[0])
                {
                    tempDisstanceField[newPos.x, newPos.y] = 0;
                }
                else
                {
                    tempDisstanceField[newPos.x, newPos.y] = newDistance;
                }

                priorityQueue.Add((newPos, newDistance));
            }


        }
        yield return null;
        //Debug.Log(n);
        DistanceField = tempDisstanceField.Clone() as int[,];
        //Debug.Log(DistanceField.GetHashCode());
        //Debug.Log(tempDisstanceField.GetHashCode());
    }

}
