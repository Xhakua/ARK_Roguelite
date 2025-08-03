using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 地图动态加载管理器
/// </summary>

public class MapDynamicLoadingManager : MonoBehaviour
{

    public enum Axis
    {
        XY,
        XZ,
        YZ
    }
    public Axis axis;
    public static MapDynamicLoadingManager Instance { get; private set; }


    public List<GameObject> pool = new List<GameObject>();
    //根节点
    [SerializeField] private Transform cubes;


    public Vector2Int dynamicMapSize = new Vector2Int(10, 10);
    public Vector3 playerOffset;
    private Vector3 playerPosition = new Vector3Int(0, 0, 0);

    private Vector2Int playerChunk;
    private Vector2Int lastPlayerChunk;

    public MapAtlasSO mapAtlas;
    private struct Sudoku
    {
        public Transform LU { get; set; }
        public Transform RU { get; set; }
        public Transform LD { get; set; }
        public Transform RD { get; set; }
        public Transform U { get; set; }
        public Transform D { get; set; }
        public Transform L { get; set; }
        public Transform R { get; set; }
        public Transform C { get; set; }

        public Transform[] All => new Transform[9] { LU, U, RU, L, C, R, LD, D, RD };

        public void Move(Vector2 dir)
        {
            if (dir == Vector2.down)
            {
                (LU, U, RU, L, C, R, LD, D, RD)
                    = (L, C, R, LD, D, RD, LU, U, RU);
            }
            else if (dir == Vector2.up)
            {
                (LU, U, RU, L, C, R, LD, D, RD)
                    = (LD, D, RD, LU, U, RU, L, C, R);
            }
            else if (dir == Vector2.right)
            {
                (LU, L, LD, U, C, D, RU, R, RD)
                    = (U, C, D, RU, R, RD, LU, L, LD);
            }
            else if (dir == Vector2.left)
            {
                (LU, L, LD, U, C, D, RU, R, RD)
                    = (RU, R, RD, LU, L, LD, U, C, D);
            }
        }

        public Transform GetTransform(Vector2 dir)
        {
            if (dir == Vector2.up)
            {
                return U;
            }
            else if (dir == Vector2.down)
            {
                return D;
            }
            else if (dir == Vector2.left)
            {
                return L;
            }
            else if (dir == Vector2.right)
            {
                return R;
            }
            else if (dir == Vector2.up + Vector2.left)
            {
                return LU;
            }
            else if (dir == Vector2.up + Vector2.right)
            {
                return RU;
            }
            else if (dir == Vector2.down + Vector2.left)
            {
                return LD;
            }
            else if (dir == Vector2.down + Vector2.right)
            {
                return RD;
            }
            else
            {
                return C;
            }
        }

        public void SetTransform(Transform transforms, Vector2 dir)
        {
            if (dir == Vector2.up)
            {
                U = transforms;
            }
            else if (dir == Vector2.down)
            {
                D = transforms;
            }
            else if (dir == Vector2.left)
            {
                L = transforms;
            }
            else if (dir == Vector2.right)
            {
                R = transforms;
            }
            else if (dir == Vector2.up + Vector2.left)
            {
                LU = transforms;
            }
            else if (dir == Vector2.up + Vector2.right)
            {
                RU = transforms;
            }
            else if (dir == Vector2.down + Vector2.left)
            {
                LD = transforms;
            }
            else if (dir == Vector2.down + Vector2.right)
            {
                RD = transforms;
            }
            else
            {
                C = transforms;
            }
        }
    }

    private Sudoku sudoku;

    private Transform[] containers = new Transform[9];

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
    private void Start()
    {
        Invoke("Init", 0.1f);
    }
    public void Init()
    {
        for (int i = 0; i < containers.Length; i++)
        {
            containers[i] = new GameObject("Container" + i).transform;
            containers[i].parent = transform;
        }

        sudoku = new Sudoku
        {
            LU = containers[0],
            U = containers[1],
            RU = containers[2],
            L = containers[3],
            C = containers[4],
            R = containers[5],
            LD = containers[6],
            D = containers[7],
            RD = containers[8]
        };


        if (GameDataManager.Instance.UseMapData)
        {
            SetPlayerSpawnPointTile();
        }

    }



    private void Instance_OnRebirth(object sender, System.EventArgs e)
    {
        SetPlayerSpawnPointTile();
    }

    public void SetPlayerSpawnPointTile()
    {
        switch (axis)
        {
            case Axis.XY:
                playerPosition = new Vector3Int((int)(MapManager.Instance.GetWorldSize().x / 2f), (int)(MapManager.Instance.GetWorldSize().y / 2f), 0) + playerOffset;
                break;
            case Axis.XZ:
                playerPosition = new Vector3Int((int)(MapManager.Instance.GetWorldSize().x / 2f), 0, (int)(MapManager.Instance.GetWorldSize().z / 2f)) + playerOffset;
                break;
            case Axis.YZ:
                playerPosition = new Vector3Int(0, (int)(MapManager.Instance.GetWorldSize().x / 2f), (int)(MapManager.Instance.GetWorldSize().y / 2f)) + playerOffset;
                break;
        }
        PlayerManager.Instance.GetPlayer().transform.position = playerPosition;
        playerChunk = Pos2Chunk(playerPosition);
        lastPlayerChunk = playerChunk;
        Vector2Int startChunk = playerChunk - Vector2Int.one;
        Vector2Int endChunk = playerChunk + Vector2Int.one;

        for (int y = startChunk.y; y <= endChunk.y; y++)
        {
            for (int x = startChunk.x; x <= endChunk.x; x++)
            {
                LoadMapChunkFast(MapManager.Instance.GetWorldData(), new Vector2Int(x, y));
                //try
                //{
                //    LoadMapChunkFast(MapManager.Instance.worldData, new Vector2Int(x, y));
                //}
                //catch (System.Exception)
                //{
                //    Debug.Log(MapManager.Instance.GetWorldData((MapManager.TileLayer)0)[x, y]);
                //    Debug.Log(MapManager.Instance.GetWorldData((MapManager.TileLayer)1)[x, y]);
                //    Debug.Log(MapManager.Instance.GetWorldData((MapManager.TileLayer)2)[x, y]);
                //    Debug.Log(x + " " + y);
                //    throw;
                //}

            }
        }
    }


    private void FixedUpdate()
    {
        if (GameDataManager.Instance.UseMapData)
        {
            playerPosition = new Vector3Int((int)PlayerManager.Instance.GetPlayer().transform.position.x, (int)PlayerManager.Instance.GetPlayer().transform.position.y, (int)PlayerManager.Instance.GetPlayer().transform.position.z);
            DynamicLoadingMap(MapManager.Instance.GetWorldData());
        }

    }

    public void DynamicLoadingMap(MapManager.WorldData mapData)
    {

        playerChunk = Pos2Chunk(playerPosition);
        if (playerChunk != lastPlayerChunk)
        {
            Vector2Int dir = playerChunk - lastPlayerChunk;
            ManageMapChunks(mapData, dir);
            lastPlayerChunk = playerChunk;

        }

    }

    private void ManageMapChunks(MapManager.WorldData mapData, Vector2Int dir)
    {
        //最终方向只有上下左右四个


        if (dir == Vector2Int.up)
        {
            UnloadMapChunkFast(Vector2.down);
            UnloadMapChunkFast(Vector2.down + Vector2.left);
            UnloadMapChunkFast(Vector2.down + Vector2.right);
            sudoku.Move(Vector2.up);
            LoadMapChunkFast(mapData, playerChunk + Vector2Int.up);
            LoadMapChunkFast(mapData, playerChunk + Vector2Int.up + Vector2Int.left);
            LoadMapChunkFast(mapData, playerChunk + Vector2Int.up + Vector2Int.right);

        }
        else if (dir == Vector2Int.down)
        {
            UnloadMapChunkFast(Vector2.up);
            UnloadMapChunkFast(Vector2.up + Vector2.left);
            UnloadMapChunkFast(Vector2.up + Vector2.right);
            sudoku.Move(Vector2.down);
            LoadMapChunkFast(mapData, playerChunk + Vector2Int.down);
            LoadMapChunkFast(mapData, playerChunk + Vector2Int.down + Vector2Int.left);
            LoadMapChunkFast(mapData, playerChunk + Vector2Int.down + Vector2Int.right);
        }
        else if (dir == Vector2Int.left)
        {
            UnloadMapChunkFast(Vector2.right);
            UnloadMapChunkFast(Vector2.up + Vector2.right);
            UnloadMapChunkFast(Vector2.down + Vector2.right);
            sudoku.Move(Vector2.left);
            LoadMapChunkFast(mapData, playerChunk + Vector2Int.left);
            LoadMapChunkFast(mapData, playerChunk + Vector2Int.left + Vector2Int.up);
            LoadMapChunkFast(mapData, playerChunk + Vector2Int.left + Vector2Int.down);
        }
        else if (dir == Vector2Int.right)
        {
            UnloadMapChunkFast(Vector2.left);
            UnloadMapChunkFast(Vector2.up + Vector2.left);
            UnloadMapChunkFast(Vector2.down + Vector2.left);
            sudoku.Move(Vector2.right);
            LoadMapChunkFast(mapData, playerChunk + Vector2Int.right);
            LoadMapChunkFast(mapData, playerChunk + Vector2Int.right + Vector2Int.up);
            LoadMapChunkFast(mapData, playerChunk + Vector2Int.right + Vector2Int.down);
        }

    }

    public void LoadMapChunkFast(MapManager.WorldData mapData, Vector2Int chunkPos)
    {
        // 根据地图数据加载地图块
        Vector2Int ldAnchor = new Vector2Int(chunkPos.x * dynamicMapSize.x, chunkPos.y * dynamicMapSize.y);
        Vector2Int ruAnchor = new Vector2Int(ldAnchor.x + dynamicMapSize.x - 1, ldAnchor.y + dynamicMapSize.y - 1);
        ldAnchor = new Vector2Int(Mathf.Max(ldAnchor.x, 0), Mathf.Max(ldAnchor.y, 0));
        ruAnchor = new Vector2Int(Mathf.Min(ruAnchor.x, mapData.worldDataFloor.GetLength(0)), Mathf.Min(ruAnchor.y, mapData.worldDataFloor.GetLength(1)));


        for (int y = ldAnchor.y; y <= ruAnchor.y; y++)
        {
            for (int x = ldAnchor.x; x <= ruAnchor.x; x++)
            {
                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        switch (axis)
                        {
                            case Axis.XY:
                                GameObject temp = GenerateCube(mapAtlas.GetTile(MapManager.Instance.GetWorldData((MapManager.TileLayer)i)[x, y]), new Vector3(x, y, i));
                                if (temp != null)
                                {
                                    Vector2Int dir = chunkPos - playerChunk;
                                    temp.transform.SetParent(sudoku.GetTransform(dir));
                                }
                                break;
                            case Axis.XZ:
                                temp = GenerateCube(mapAtlas.GetTile(MapManager.Instance.GetWorldData((MapManager.TileLayer)i)[x, y]), new Vector3(x, i, y));
                                if (temp != null)
                                {
                                    Vector2Int dir = chunkPos - playerChunk;
                                    temp.transform.SetParent(sudoku.GetTransform(dir));
                                }
                                break;
                            case Axis.YZ:
                                temp = GenerateCube(mapAtlas.GetTile(MapManager.Instance.GetWorldData((MapManager.TileLayer)i)[x, y]), new Vector3(i, x, y));
                                if (temp != null)
                                {
                                    Vector2Int dir = chunkPos - playerChunk;
                                    temp.transform.SetParent(sudoku.GetTransform(dir));
                                }
                                break;
                        }
                    }
                    catch (System.Exception)
                    {
                        Debug.LogError(x + " " + y + " " + i);
                        Debug.LogError((MapManager.TileLayer)i);
                        Debug.LogError(MapManager.Instance.GetWorldSize());
                        Debug.LogError(MapManager.Instance.GetWorldData((MapManager.TileLayer)i)[x, y]);

                        throw;
                    }

                }
                //GameObject temp = GenerateCube(mapAtlas.GetCube(MapManager.Instance.GetWorldData((MapManager.TileLayer)1)[x, y]), new Vector3(x, y, -1));
                //if (temp != null)
                //{
                //    Vector2Int faceDir = chunkPos - playerChunk;
                //    temp.transform.SetParent(sudoku.GetTransform(faceDir));
                //}
            }
        }
    }

    private void UnloadMapChunkFast(Vector2 dir)
    {
        int index = 0;
        while (sudoku.GetTransform(dir).childCount > 0)
        {
            GameObject temp = sudoku.GetTransform(dir).GetChild(0).gameObject;
            temp.transform.SetParent(cubes);
            temp.SetActive(false);
            pool.Add(temp);
            index++;
            if (index >= 256)
            {
                break;
            }
        }
    }

    /// <summary>
    /// 销毁指定位置的方块
    /// 没有写入地图数据。
    /// 建议使用时，先调用MapManager.Instance.SetWorldData()写入地图数据。
    /// </summary>
    /// <param name="pos"></param>
    public void DestroyCube(Vector2Int pos)
    {
        // 在指定位置生成一个小球，检测所有碰撞体
        Collider[] colliders = Physics.OverlapSphere(new Vector3(pos.x, 1, pos.y), 0.2f, LayerMask.GetMask("Wall"));
        foreach (var collider in colliders)
        {
            collider.gameObject.SetActive(false);
            collider.gameObject.transform.SetParent(cubes);
            pool.Add(collider.gameObject);
        }
    }


    public List<GameObject> GetCubeBlock(Vector2Int startPos, Vector2Int endPos)
    {
        Vector2Int anchorsLD = new Vector2Int(Mathf.Min(startPos.x, endPos.x), Mathf.Min(startPos.y, endPos.y));
        Vector2Int anchorsRU = new Vector2Int(Mathf.Max(startPos.x, endPos.x), Mathf.Max(startPos.y, endPos.y));
        List<GameObject> cubes = new List<GameObject>();
        Collider[] colliders = Physics.OverlapBox(new Vector3((anchorsLD.x + anchorsRU.x) / 2f,
            2,
            (anchorsLD.y + anchorsRU.y) / 2f),
            new Vector3((anchorsRU.x - anchorsLD.x) / 2f,
            2,
            (anchorsRU.y - anchorsLD.y) / 2f),
            Quaternion.identity,
            LayerMask.GetMask("Wall"));
        foreach (var collider in colliders)
        {
            cubes.Add(collider.gameObject);
        }
        return cubes;

    }


    /// <summary>
    /// 生成一个方块
    /// 没有写入地图数据。
    /// 建议使用时，先调用MapManager.Instance.SetWorldData()写入地图数据。
    /// </summary>
    /// <param name="cube"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public GameObject GenerateCube(GameObject cube, Vector3 targetPos)
    {
        if (cube == null)
        {
            return null;
        }
        bool takeCube = false;
        GameObject ret = null;


        foreach (GameObject c in pool)
        {
            if ((c.name) == cube.name + "(Clone)")
            {
                c.transform.position = targetPos;
                c.SetActive(true);
                //c.GetComponent<Move2Pos>().Move(targetPos);
                takeCube = true;
                ret = c;
                pool.Remove(c);
                break;
            }
        }

        if (!takeCube)
        {
            ret = Instantiate(cube, targetPos, Quaternion.identity);
            //ret.GetComponent<SpriteRenderer>().enabled = false;
            //ret.GetComponent<Move2Pos>().Move(targetPos);

        }



        return ret;
    }

    /// <summary>
    /// 生成一个矩形方块区域
    /// 没有写入地图数据。
    /// 建议使用时，先调用MapManager.Instance.SetWorldData()写入地图数据。
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="cubeSO"></param>
    public List<GameObject> GenerateCubeBlock(Vector3Int startPos, Vector3Int endPos, CubeSO cubeSO)
    {
        Vector2Int anchorsLD = new Vector2Int(Mathf.Min(startPos.x, endPos.x), Mathf.Min(startPos.z, endPos.z));
        Vector2Int anchorsRU = new Vector2Int(Mathf.Max(startPos.x, endPos.x), Mathf.Max(startPos.z, endPos.z));
        List<GameObject> generatedCubes = new List<GameObject>();
        for (int y = anchorsLD.y; y <= anchorsRU.y; y++)
        {
            for (int x = anchorsLD.x; x <= anchorsRU.x; x++)
            {
                if (!MapManager.Instance.CanPlaceTile(new Vector3Int(x, y, 0), MapManager.TileLayer.Wall, cubeSO.id))
                {
                    continue;
                }

                GameObject temp = GenerateCube(cubeSO.cube, new Vector3(x, 1, y));
                generatedCubes.Add(temp);
            }
        }
        return generatedCubes;
    }

    public Vector2Int Pos2Chunk(Vector2Int pos)
    {
        return Pos2Chunk(new Vector3(pos.x, 0, pos.y));
    }
    public Vector2Int Pos2Chunk(Vector3 pos)
    {
        switch (axis)
        {
            case Axis.XY:
                return Pos2ChunkXY(pos);
            case Axis.XZ:
                return Pos2ChunkXZ(pos);
            case Axis.YZ:
                return Pos2ChunkYZ(pos);
            default:
                return Pos2ChunkXY(pos);
        }
    }
    private Vector2Int Pos2ChunkXY(Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        return new Vector2Int(x / dynamicMapSize.x, y / dynamicMapSize.y);
    }
    private Vector2Int Pos2ChunkXZ(Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.z;
        return new Vector2Int(x / dynamicMapSize.x, y / dynamicMapSize.y);
    }
    private Vector2Int Pos2ChunkYZ(Vector3 pos)
    {
        int x = (int)pos.y;
        int y = (int)pos.z;
        return new Vector2Int(x / dynamicMapSize.x, y / dynamicMapSize.y);
    }
}
