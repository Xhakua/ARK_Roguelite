
using System;
using UnityEngine;
public class MapManager : MonoBehaviour
{
    //泰拉瑞亚世界4200*1200 (6400*1800) 8400*2400
    //200*200=35*35
    //6400*1800=1120*320

    public static MapManager Instance { get; private set; }

    [Serializable]
    public struct WorldData
    {
        public int[,] worldDataFloor;

        public int[,] worldDataWall;

    }



    private WorldData worldData;
    public int[,] lightData;

    public enum TileLayer
    {
        Floor,
        Wall,
        Light
    }


    /// <summary>
    /// 4=Honkai
    /// data=0是空的
    /// 获取时手动加一个偏移量-1
    /// 因为0是空的
    /// 但是物品的ID是从0开始的
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int[,] GetWorldData(TileLayer tileLayer)
    {
        //FIXME: 能够自动加偏移量
        int offset = -1;
        switch (tileLayer)
        {
            case TileLayer.Floor:
                return worldData.worldDataFloor ;
            case TileLayer.Wall:
                return worldData.worldDataWall ;
            case TileLayer.Light:
                return lightData;
            default:
                return null;
        }
    }

    /// <summary>
    /// 获取世界数据
    /// data=0是空的
    /// 获取时手动加一个偏移量-1
    /// 因为0是空的
    /// </summary>
    /// <returns></returns>
    public WorldData GetWorldData()
    {
        return worldData;
    }

    public bool isDreamland = false;
    private Vector3Int worldSize = new Vector3Int(100, 100);
    [SerializeField] private PerlinMapSO mapChunk;

    [SerializeField] private float scale = 0.18f;
    [SerializeField, Range(0, 1)] private float caveThreshold = 0.5f;
    [SerializeField, Range(0, 1)] private float caveScale;
    [SerializeField, Range(0, 5)] public int smoothTurn;
    [SerializeField] private int seed = 0;


    public MapAtlasSO mapAtlas;
    [InspectorButton("Generate")] public bool generate = false;


    public int[,] caveData;//1:空气 0:石头

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

    //private void Start()
    //{
    //    Init();
    //}
    public void Init()
    {
        //worldSize.z
        worldSize = mapChunk.worldSize;
        caveData = new int[worldSize.x, worldSize.z];
        WorldDataInit(worldSize);
        //Debug.Log(worldData.worldDataFloor);
        //GetComponent<WaveFunctionCollapseGenerator>().InitializeGeneration();
        GenerateMap();

    }


    /// <summary>
    /// 有Z
    /// </summary>
    /// <returns></returns>
    public Vector3Int GetWorldSize()
    {
        return worldSize;
    }


    public void Generate()//按钮调用
    {
        seed = 0;
        worldSize = mapChunk.worldSize;
        caveData = new int[worldSize.x, worldSize.z];
        Debug.Log(worldSize);


        generate = !generate;
        if (generate)
        {
            GenerateMap();

        }

    }

    public PerlinMapSO GetMapChunk()
    {
        return mapChunk;
    }
    public Color GetColor(int x, int y, TileLayer tileLayer)
    {
        int tileID = GetWorldData(tileLayer)[x, y];
        if (tileID == 0)
        {
            return Color.black;
        }
        return mapAtlas.GetColor(tileID);
    }

    public void GenerateMap()
    {

        seed = UnityEngine.Random.Range(0, 100000);




        //wall
        for (int x = 0; x < worldSize.x; x++)
        {
            for (int y = 0; y < worldSize.z; y++)
            {
                float perlinN = Mathf.PerlinNoise(seed + (float)x * scale, seed + (float)y * scale);
                if (perlinN > caveThreshold)
                {
                    SetWorldData(new Vector3Int(x, y, 2), TileLayer.Wall, mapChunk.wall.id);

                }
                else
                {
                    caveData[x, y] = 1;
                    SetWorldData(new Vector3Int(x, y, 2), TileLayer.Wall, 0);
                }

            }
        }
        //floor
        for (int x = 0; x < worldSize.x; x++)
        {
            for (int y = 0; y < worldSize.z; y++)
            {
                SetWorldData(new Vector3Int(x, y, 2), TileLayer.Floor, mapChunk.floor.id);
            }
        }



        for (int x = 0; x < worldSize.x; x++)
        {
            for (int y = 0; y < worldSize.z; y++)
            {
                for (int z = 0; z < mapChunk.mines.Length; z++)
                {
                    PerlinMapSO.Mine mine = mapChunk.mines[z];
                    float perlinN = 1f - Mathf.PerlinNoise(mine.seed + (float)x * (mine.frequency + 0.1f), mine.seed + (float)y * (mine.frequency + 0.1f));
                    if (caveData[x, y] == 0 && perlinN < mine.abundance)
                    {
                        SetWorldData(new Vector3Int(x, y, 2), TileLayer.Wall, mine.mine.id);
                    }
                }
            }
        }

        //smooth
        for (int turn = 0; turn < smoothTurn; turn++)
        {
            for (int x = 0; x < worldSize.x; x++)
            {
                for (int y = 0; y < worldSize.z; y++)
                {


                    int count = CountSurroundingTiles(x, y);

                    if (count <= 2)
                    {
                        //tilemaps[2].SetTile(new Vector3Int(x, y, 0), GetMapChunk(x).stoneTile);//stone
                        SetWorldData(new Vector3Int(x, y, 2), TileLayer.Wall, mapChunk.wall.id);
                        //caveData[x, y] = 0;
                    }
                    else
                    if (count >= 5)
                    {
                        //tilemaps[2].SetTile(new Vector3Int(x, y, 0), null); // air
                        SetWorldData(new Vector3Int(x, y, 2), TileLayer.Wall, 0);
                        //caveData[x, y] = 1;
                    }
                    else
                    {
                        //tilemaps[2].SetTile(new Vector3Int(x, y, 0), GetMapChunk(x).stoneTile);//stone
                        SetWorldData(new Vector3Int(x, y, 2), TileLayer.Wall, mapChunk.wall.id);
                        //caveData[x, y] = 0;
                    }
                }
            }
        }
        Debug.Log("生成地图完成" + worldSize);
    }




    private int CountSurroundingTiles(int x, int y)
    {
        int count = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int neighborX = Mathf.Clamp(x + dx, 0, worldSize.x - 1);
                int neighborY = Mathf.Clamp(y + dy, 0, worldSize.z - 1);

                // 不考虑中心点自身
                if (dx == 0 && dy == 0)
                {
                    continue;
                }

                // 如果相邻位置有是空气，增加计数
                if (caveData[neighborX, neighborY] == 1)
                {
                    count++;
                }
            }
        }
        return count;
    }


    /// <summary>
    /// 检查是否可以放置tile
    /// z没有用
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="layer"></param>
    /// <param name="tileID"></param>
    /// <returns></returns>
    public bool CanPlaceTile(Vector3Int pos, TileLayer layer, int tileID)
    {
        // 检查位置是否在世界范围内
        if (pos.x < 0 || pos.x >= worldSize.x || pos.y < 0 || pos.y >= worldSize.z)
        {
            return false;
        }
        if (tileID == 0)
        {
            return true; // tileID==0表示要删除方块，可以放置
        }
        // 检查是否已经有tile存在
        if (GetWorldData(pos, layer) != 0)
        {
            return false; // 已经有tile存在，不能覆盖
        }
        return true; // 可以放置tile
    }

    /// <summary>
    /// z没有用
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="layer"></param>
    /// <param name="tileID"></param>
    public void SetWorldData(Vector3Int pos, TileLayer layer, int tileID)
    {
        if (layer == TileLayer.Light)
        {
            pos.x = Mathf.Clamp(pos.x, 0, worldSize.x - 1);
            pos.y = Mathf.Clamp(pos.y, 0, worldSize.z - 1);
            GetWorldData(layer)[pos.x, pos.y] += tileID;
        }


        pos.x = Mathf.Clamp(pos.x, 0, worldSize.x - 1);
        pos.y = Mathf.Clamp(pos.y, 0, worldSize.z - 1);
        //Debug.Log(pos);
        //Debug.Log(tileID);
        //Debug.Log(layer);
        GetWorldData(layer)[pos.x, pos.y] = tileID;

    }

    public void SetWorldDataBlock(Vector3Int startPos, Vector3Int endPos, TileLayer layer, int tileID)
    {
        Vector2Int anchorsLD = new Vector2Int(Mathf.Min(startPos.x, endPos.x), Mathf.Min(startPos.z, endPos.z));
        Vector2Int anchorsRU = new Vector2Int(Mathf.Max(startPos.x, endPos.x), Mathf.Max(startPos.z, endPos.z));

        for (int x = anchorsLD.x; x <= anchorsRU.x; x++)
        {
            for (int y = anchorsLD.y; y <= anchorsRU.y; y++)
            {
                if (!CanPlaceTile(new Vector3Int(x, y, 2), layer, tileID))
                {
                    continue; // 如果不能放置tile，则跳过
                }
                SetWorldData(new Vector3Int(x, y, 2), layer, tileID);
            }
        }
    }

    public void SetWorldData(Vector3Int pos, ChunkPartSO chunkPartSO)
    {
        int[,,] data = chunkPartSO.Data;
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                for (int k = 0; k < data.GetLength(2); k++)
                {
                    SetWorldData(new Vector3Int(i, j, k), (TileLayer)k, data[i, j, k]);
                }
            }
        }
    }

    public int GetWorldData(Vector3Int pos, TileLayer layer)
    {
        pos.x = Mathf.Clamp(pos.x, 0, worldSize.x - 1);
        pos.y = Mathf.Clamp(pos.y, 0, worldSize.z - 1);

        return GetWorldData(layer)[pos.x, pos.y];

    }

    /// <summary>
    /// pos2到pos1
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    public void ExchangeLightData(int lightIntensity, Vector3Int currentPos, Vector3Int targetPos)
    {
        //Debug.Log(tempTileID);
        SetWorldData(targetPos, TileLayer.Light, lightIntensity);

        SetWorldData(currentPos, TileLayer.Light, -lightIntensity);
    }
    public void WorldDataInit(Vector3Int worldSize)
    {
        worldData.worldDataFloor = new int[worldSize.x, worldSize.z];
        worldData.worldDataWall = new int[worldSize.x, worldSize.z];
        lightData = new int[worldSize.x, worldSize.z];
        Debug.Log(worldSize);
        for (int i = 0; worldSize.x > i; i++)
        {
            for (int j = 0; worldSize.z > j; j++)
            {
                worldData.worldDataFloor[i, j] = 0;
                worldData.worldDataWall[i, j] = 0;
                lightData[i, j] = -1;
            }
        }



    }
}
